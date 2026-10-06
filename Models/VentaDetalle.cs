namespace TiendaApp.Models;

public class VentaDetalle
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = "";
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }

    public decimal Subtotal => PrecioUnitario * Cantidad;
}
