using System.IO;
using Microsoft.EntityFrameworkCore;

using TiendaApp.Models;

namespace TiendaApp.Data;

public class TiendaContext : DbContext
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<VentaDetalle> VentaDetalles => Set<VentaDetalle>();

    // La base se guarda en %LocalAppData%\TiendaApp\tienda.db
    public static string RutaDb
    {
        get
        {
            var carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TiendaApp");
            Directory.CreateDirectory(carpeta);
            return Path.Combine(carpeta, "tienda.db");
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={RutaDb}");
}
