using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TiendaApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object? actual;

    public MainViewModel() => IrCobrar();

    // Se crea un ViewModel nuevo en cada navegación para que siempre lea datos frescos.
    [RelayCommand]
    private void IrCobrar() => Actual = new VentasViewModel();

    [RelayCommand]
    private void IrProductos() => Actual = new ProductosViewModel();
}
