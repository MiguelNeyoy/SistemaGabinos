using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace SistemaGabinos.Views;

public partial class PanelDeControl : Page
{
    private readonly ViewModels.PanelDeControlViewModel _viewModel;

    public PanelDeControl(ViewModels.PanelDeControlViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Loaded += (s, e) => _viewModel.CargarMetricas();
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
