using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TiendaApp.Data;
using TiendaApp.Models;

namespace TiendaApp.ViewModels;

public partial class ProductosViewModel : ObservableObject
{
    public ObservableCollection<Producto> Productos { get; } = new();

    [ObservableProperty] private Producto? seleccionado;
    [ObservableProperty] private string nombre = "";
    [ObservableProperty] private string categoria = "";
    [ObservableProperty] private decimal precio;
    [ObservableProperty] private int stock;

    public ProductosViewModel() => Cargar();

    private void Cargar()
    {
        Productos.Clear();
        using var db = new TiendaContext();
        foreach (var p in db.Productos.AsEnumerable().OrderBy(p => p.Nombre))
            Productos.Add(p);
    }

    partial void OnSeleccionadoChanged(Producto? value)
    {
        if (value is null) return;
        Nombre = value.Nombre;
        Categoria = value.Categoria;
        Precio = value.Precio;
        Stock = value.Stock;
    }

    [RelayCommand]
    private void Guardar()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            MessageBox.Show("Escribe el nombre del producto.", "Productos");
            return;
        }
        if (Precio < 0 || Stock < 0)
        {
            MessageBox.Show("El precio y el stock no pueden ser negativos.", "Productos");
            return;
        }

        using (var db = new TiendaContext())
        {
            if (Seleccionado is null)
            {
                db.Productos.Add(new Producto
                {
                    Nombre = Nombre.Trim(), Categoria = Categoria.Trim(), Precio = Precio, Stock = Stock
                });
            }
            else
            {
                var p = db.Productos.Find(Seleccionado.Id);
                if (p is not null)
                {
                    p.Nombre = Nombre.Trim();
                    p.Categoria = Categoria.Trim();
                    p.Precio = Precio;
                    p.Stock = Stock;
                }
            }
            db.SaveChanges();
        }

        Limpiar();
        Cargar();
    }

    [RelayCommand]
    private void Eliminar()
    {
        if (Seleccionado is null) return;

        var r = MessageBox.Show($"¿Eliminar \"{Seleccionado.Nombre}\"?", "Productos",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (r != MessageBoxResult.Yes) return;

        using (var db = new TiendaContext())
        {
            var p = db.Productos.Find(Seleccionado.Id);
            if (p is not null)
            {
                db.Productos.Remove(p);
                db.SaveChanges();
            }
        }

        Limpiar();
        Cargar();
    }

    [RelayCommand]
    private void Limpiar()
    {
        Seleccionado = null;
        Nombre = "";
        Categoria = "";
        Precio = 0;
        Stock = 0;
    }
}
