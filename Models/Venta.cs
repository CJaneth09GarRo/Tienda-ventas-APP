namespace TiendaApp.Models;

public class Venta
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public decimal Pago { get; set; }
    public decimal Cambio { get; set; }
    public List<VentaDetalle> Detalles { get; set; } = new();
}
