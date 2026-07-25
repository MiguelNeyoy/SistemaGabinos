using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaGabinos.ViewModels;

namespace SistemaGabinos.Views;

public partial class Configuracion : Page
{
    public ConfiguracionViewModel ViewModel { get; }

    public Configuracion()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ConfiguracionViewModel>();
        DataContext = ViewModel;

        Loaded += (s, e) => ViewModel.CargarPrecios();
    }
}
