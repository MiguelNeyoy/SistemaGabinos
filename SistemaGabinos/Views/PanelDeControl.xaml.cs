using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace SistemaGabinos.Views;

public partial class PanelDeControl : Page
{
    public PanelDeControl()
    {
        InitializeComponent();
    }

    private void NuevaMatricula_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        NavigationService.Navigate(App.Services.GetRequiredService<NuevaMatricula>());
    }

    private void Pagos_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        NavigationService.Navigate(App.Services.GetRequiredService<VentanillaCobro>());
    }
}
