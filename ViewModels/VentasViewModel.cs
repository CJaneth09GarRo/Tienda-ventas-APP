using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TiendaApp.Data;
using TiendaApp.Models;
using TiendaApp.Services;
using TiendaApp.Views;

namespace TiendaApp.ViewModels;

public partial class VentasViewModel : ObservableObject
{
    private List<Producto> _todos = new();

    public ObservableCollection<Producto> Catalogo { get; } = new();
    public ObservableCollection<ItemCarrito> Carrito { get; } = new();

    [ObservableProperty]
    private string busqueda = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PagoCon))]
    [NotifyPropertyChangedFor(nameof(Cambio))]
    [NotifyCanExecuteChangedFor(nameof(CobrarCommand))]
    private string pagoTexto = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Cambio))]
    [NotifyCanExecuteChangedFor(nameof(CobrarCommand))]
    private decimal total;

    public decimal PagoCon =>
        decimal.TryParse(PagoTexto, NumberStyles.Any, CultureInfo.CurrentCulture, out var v) ? v : 0m;

    public decimal Cambio => Math.Max(0m, PagoCon - Total);

    public int TotalArticulos => Carrito.Sum(i => i.Cantidad);

    public VentasViewModel() => CargarCatalogo();

    // ---------- Catálogo ----------
    private void CargarCatalogo()
    {
        using var db = new TiendaContext();
        _todos = db.Productos.AsEnumerable().OrderBy(p => p.Nombre).ToList();
        Filtrar();
    }

    partial void OnBusquedaChanged(string value) => Filtrar();

    private void Filtrar()
    {
        var q = (Busqueda ?? "").Trim();
        Catalogo.Clear();
        foreach (var p in _todos.Where(p => p.Stock > 0 &&
                     (q.Length == 0 ||
                      p.Nombre.Contains(q, StringComparison.CurrentCultureIgnoreCase) ||
                      p.Categoria.Contains(q, StringComparison.CurrentCultureIgnoreCase))))
            Catalogo.Add(p);
    }

    // ---------- Carrito ----------
    [RelayCommand]
    private void Agregar(Producto? p)
    {
        if (p is null) return;

        var item = Carrito.FirstOrDefault(i => i.Producto.Id == p.Id);
        if (item is null)
            Carrito.Add(new ItemCarrito(p));
        else if (item.Cantidad < p.Stock)
            item.Cantidad++;
        else
        {
            MessageBox.Show($"No hay más existencias de \"{p.Nombre}\".", "Cobro");
            return;
        }
        Recalcular();
    }

    [RelayCommand]
    private void Mas(ItemCarrito? item)
    {
        if (item is null) return;
        if (item.Cantidad >= item.Producto.Stock)
        {
            MessageBox.Show($"No hay más existencias de \"{item.Producto.Nombre}\".", "Cobro");
            return;
        }
        item.Cantidad++;
        Recalcular();
    }

    [RelayCommand]
    private void Menos(ItemCarrito? item)
    {
        if (item is null) return;
        if (item.Cantidad > 1) item.Cantidad--;
        else Carrito.Remove(item);
        Recalcular();
    }

    [RelayCommand]
    private void Quitar(ItemCarrito? item)
    {
        if (item is null) return;
        Carrito.Remove(item);
        Recalcular();
    }

    [RelayCommand]
    private void Vaciar()
    {
        Carrito.Clear();
        PagoTexto = "";
        Recalcular();
    }

    [RelayCommand]
    private void PagoExacto() => PagoTexto = Total.ToString("0.00", CultureInfo.CurrentCulture);

    private void Recalcular()
    {
        Total = Carrito.Sum(i => i.Subtotal);
        OnPropertyChanged(nameof(TotalArticulos));
        OnPropertyChanged(nameof(Cambio));
        CobrarCommand.NotifyCanExecuteChanged();
    }

    // ---------- Cobro ----------
    private bool PuedeCobrar() => Carrito.Count > 0 && Total > 0 && PagoCon >= Total;

    [RelayCommand(CanExecute = nameof(PuedeCobrar))]
    private void Cobrar()
    {
        Venta venta;

        using (var db = new TiendaContext())
        using (var tx = db.Database.BeginTransaction())
        {
            venta = new Venta
            {
                Fecha = DateTime.Now,
                Total = Total,
                Pago = PagoCon,
                Cambio = Cambio
            };

            foreach (var item in Carrito)
            {
                var prod = db.Productos.Find(item.Producto.Id);
                if (prod is null || prod.Stock < item.Cantidad)
                {
                    MessageBox.Show($"Existencias insuficientes de \"{item.Producto.Nombre}\".", "Cobro");
                    return;   // al salir sin Commit, la transacción se revierte
                }

                prod.Stock -= item.Cantidad;
                venta.Detalles.Add(new VentaDetalle
                {
                    ProductoId = prod.Id,
                    Nombre = prod.Nombre,
                    PrecioUnitario = item.Producto.Precio,
                    Cantidad = item.Cantidad
                });
            }

            db.Ventas.Add(venta);
            db.SaveChanges();
            tx.Commit();
        }

        var texto = TicketService.Generar(venta);
        var carpeta = TicketService.Guardar(venta, texto);

        // Nueva cuenta lista para el siguiente cliente
        Carrito.Clear();
        PagoTexto = "";
        Recalcular();
        CargarCatalogo();

        new TicketWindow(texto, carpeta) { Owner = Application.Current.MainWindow }.ShowDialog();
    }
}
