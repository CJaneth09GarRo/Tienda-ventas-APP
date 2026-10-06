using System.Windows;
using TiendaApp.Data;
using TiendaApp.Models;

namespace TiendaApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // La base de datos debe existir ANTES de abrir la ventana principal.
        using (var db = new TiendaContext())
        {
            db.Database.EnsureCreated();

            if (!db.Productos.Any())
            {
                db.Productos.AddRange(
                    new Producto { Nombre = "Refresco 600 ml", Categoria = "Bebidas", Precio = 18, Stock = 48 },
                    new Producto { Nombre = "Agua 1 L", Categoria = "Bebidas", Precio = 12, Stock = 60 },
                    new Producto { Nombre = "Papas fritas", Categoria = "Botanas", Precio = 22.5m, Stock = 30 },
                    new Producto { Nombre = "Galletas", Categoria = "Botanas", Precio = 15, Stock = 40 },
                    new Producto { Nombre = "Pan de caja", Categoria = "Abarrotes", Precio = 45, Stock = 15 },
                    new Producto { Nombre = "Leche 1 L", Categoria = "Lácteos", Precio = 27, Stock = 25 });
                db.SaveChanges();
            }
        }

        base.OnStartup(e);
    }
}
