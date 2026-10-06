# Mi Tienda – App de escritorio (WPF + C# + SQLite)

## Requisitos
- Windows 10/11
- SDK de .NET 8  (https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio Code con la extensión "C# Dev Kit"

## Ejecutar
```
cd TiendaApp
dotnet restore
dotnet run
```

## Qué incluye
- **Cobrar**: catálogo con búsqueda, cuenta con cantidades, total, "paga con", cambio y ticket.
  Al cobrar descuenta el stock, guarda la venta y muestra el ticket (imprimir / guardar .txt).
- **Productos**: CRUD completo (alta, edición, baja).

## Dónde se guardan los datos
- Base de datos: `%LocalAppData%\TiendaApp\tienda.db`
- Tickets: `Documentos\TiendaApp\Tickets\ticket_00001.txt`

## Personalizar
- Nombre del negocio en el ticket: `Services/TicketService.cs` (`NombreTienda`).
- Colores: `App.xaml` (`PrimaryColor`, `SecondaryColor`, `BaseTheme` = Dark o Light).
