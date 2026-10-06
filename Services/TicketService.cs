using System.Globalization;
using System.IO;
using System.Text;
using TiendaApp.Models;

namespace TiendaApp.Services;

public static class TicketService
{
    public const string NombreTienda = "MI TIENDA";   // <- cambia el nombre de tu negocio aquí
    private const int Ancho = 40;
    private static readonly CultureInfo Mx = new("es-MX");

    public static string Generar(Venta v)
    {
        var sb = new StringBuilder();
        var linea = new string('-', Ancho);
        var doble = new string('=', Ancho);

        sb.AppendLine(Centrar(NombreTienda));
        sb.AppendLine(Centrar("Ticket de venta"));
        sb.AppendLine(doble);
        sb.AppendLine($"Folio: {v.Id:D5}");
        sb.AppendLine($"Fecha: {v.Fecha:dd/MM/yyyy HH:mm}");
        sb.AppendLine(linea);
        sb.AppendLine("CANT".PadRight(5) + "PRODUCTO".PadRight(18) + "P.UNIT".PadLeft(8) + "IMPORTE".PadLeft(9));
        sb.AppendLine(linea);

        foreach (var d in v.Detalles)
        {
            sb.AppendLine(
                d.Cantidad.ToString().PadRight(5) +
                Cortar(d.Nombre, 17).PadRight(18) +
                Dinero(d.PrecioUnitario).PadLeft(8) +
                Dinero(d.Subtotal).PadLeft(9));
        }

        sb.AppendLine(linea);
        sb.AppendLine(Derecha("ARTICULOS:", v.Detalles.Sum(d => d.Cantidad).ToString()));
        sb.AppendLine(Derecha("TOTAL:", "$" + Dinero(v.Total)));
        sb.AppendLine(Derecha("PAGO:", "$" + Dinero(v.Pago)));
        sb.AppendLine(Derecha("CAMBIO:", "$" + Dinero(v.Cambio)));
        sb.AppendLine(doble);
        sb.AppendLine(Centrar("¡Gracias por su compra!"));
        sb.AppendLine(Centrar("Vuelva pronto"));

        return sb.ToString();
    }

    public static string Guardar(Venta v, string texto)
    {
        var carpeta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TiendaApp", "Tickets");
        Directory.CreateDirectory(carpeta);
        File.WriteAllText(Path.Combine(carpeta, $"ticket_{v.Id:D5}.txt"), texto, Encoding.UTF8);
        return carpeta;
    }

    private static string Dinero(decimal m) => m.ToString("N2", Mx);

    private static string Cortar(string s, int max) => s.Length <= max ? s : s[..max];

    private static string Centrar(string s) => s.PadLeft((Ancho + s.Length) / 2);

    private static string Derecha(string etiqueta, string valor)
        => etiqueta + valor.PadLeft(Math.Max(valor.Length, Ancho - etiqueta.Length));
}
