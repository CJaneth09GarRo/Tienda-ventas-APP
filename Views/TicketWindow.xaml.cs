using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;

namespace TiendaApp.Views;

public partial class TicketWindow : Window
{
    private readonly string _texto;
    private readonly string _carpeta;

    public TicketWindow(string texto, string carpeta)
    {
        InitializeComponent();
        _texto = texto;
        _carpeta = carpeta;
        TxtTicket.Text = texto;
    }

    private void Imprimir_Click(object sender, RoutedEventArgs e)
    {
        var doc = new FlowDocument
        {
            FontFamily = new FontFamily("Consolas"),
            FontSize = 12,
            PagePadding = new Thickness(40)
        };

        foreach (var linea in _texto.Split('\n'))
        {
            var l = linea.TrimEnd('\r');
            doc.Blocks.Add(new Paragraph(new Run(l.Length == 0 ? " " : l)) { Margin = new Thickness(0) });
        }

        var dialogo = new PrintDialog();
        if (dialogo.ShowDialog() == true)
        {
            doc.PageHeight = dialogo.PrintableAreaHeight;
            doc.PageWidth = dialogo.PrintableAreaWidth;
            dialogo.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Ticket de venta");
        }
    }

    private void Carpeta_Click(object sender, RoutedEventArgs e)
        => Process.Start("explorer.exe", $"\"{_carpeta}\"");

    private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();
}
