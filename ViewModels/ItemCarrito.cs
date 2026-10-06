using CommunityToolkit.Mvvm.ComponentModel;
using TiendaApp.Models;

namespace TiendaApp.ViewModels;

public partial class ItemCarrito : ObservableObject
{
    public Producto Producto { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Subtotal))]
    private int cantidad = 1;

    public decimal Subtotal => Producto.Precio * Cantidad;

    public ItemCarrito(Producto producto) => Producto = producto;
}
