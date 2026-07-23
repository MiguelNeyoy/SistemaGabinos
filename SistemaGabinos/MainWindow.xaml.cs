using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SistemaGabinos.ViewModels;
using SistemaGabinos.Views;

namespace SistemaGabinos;

public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MainWindowViewModel>();
        DataContext = ViewModel;

        ViewModel.NavegarAExpedienteSolicitado += NavegarAExpediente;

        Loaded += (s, e) => PrimaryContainer.Navigate(
            App.Services.GetRequiredService<PanelDeControl>()
        );
    }

    private void NavegarAExpediente(int alumnoId)
    {
        var page = App.Services.GetRequiredService<ExpedienteAlumno>();
        if (page.DataContext is ExpedienteAlumnoViewModel vm)
        {
            vm.CargarAlumno(alumnoId);
        }
        PrimaryContainer.Navigate(page);
    }

    private void FunctionPanel_Click(object sender, RoutedEventArgs e)
    {
        PrimaryContainer.Navigate(App.Services.GetRequiredService<PanelDeControl>());
    }

    private void NewEnrollment(object sender, RoutedEventArgs e)
    {
        PrimaryContainer.Navigate(App.Services.GetRequiredService<NuevaMatricula>());
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
    }
}
