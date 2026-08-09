using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using SistemaGabinos.ViewModels;
using SistemaGabinos.Views;

namespace SistemaGabinos;

public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel { get; }
    private int? _alumnoIdActivo;

    public MainWindow()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<MainWindowViewModel>();
        DataContext = ViewModel;

        ViewModel.NavegarAExpedienteSolicitado += NavegarAExpediente;
        SearchBox.SearchAccepted += (s, e) => ViewModel.SearchAcceptedCommand.Execute(null);

        // Atajo global tecla F9 para abrir la Ventanilla de Cobro Exprés
        PreviewKeyDown += MainWindow_PreviewKeyDown;

        Loaded += (s, e) => PrimaryContainer.Navigate(
            App.Services.GetRequiredService<PanelDeControl>()
        );
    }

    private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F9)
        {
            e.Handled = true;
            NavegarACobro(_alumnoIdActivo);
        }
    }

    private void NavegarAExpediente(int alumnoId)
    {
        _alumnoIdActivo = alumnoId;
        var page = App.Services.GetRequiredService<ExpedienteAlumno>();
        if (page.DataContext is ExpedienteAlumnoViewModel vm)
        {
            vm.CargarAlumno(alumnoId);
            vm.NavegarACobroSolicitado += (id, deudaId) => NavegarACobro(id, deudaId);
        }
        PrimaryContainer.Navigate(page);
    }

    private void NavegarACobro(int? alumnoId = null, int? deudaId = null)
    {
        var page = App.Services.GetRequiredService<VentanillaCobro>();
        if (alumnoId.HasValue)
        {
            page.CargarDatosAlumno(alumnoId.Value, deudaId);
        }

        page.ViewModel.CobroCompletado += () => RetornarTrasCobro(alumnoId);
        page.ViewModel.CancelarSolicitado += () => RetornarTrasCobro(alumnoId);

        PrimaryContainer.Navigate(page);
    }

    private void RetornarTrasCobro(int? alumnoId)
    {
        if (alumnoId.HasValue)
        {
            NavegarAExpediente(alumnoId.Value);
        }
        else
        {
            PrimaryContainer.Navigate(App.Services.GetRequiredService<PanelDeControl>());
        }
    }

    private void FunctionPanel_Click(object sender, RoutedEventArgs e)
    {
        PrimaryContainer.Navigate(App.Services.GetRequiredService<PanelDeControl>());
    }

    private void NewEnrollment(object sender, RoutedEventArgs e)
    {
        var page = App.Services.GetRequiredService<NuevaMatricula>();
        if (page.DataContext is NuevaMatriculaViewModel vm)
        {
            vm.NavegarACobroSolicitado += (alumnoId) => NavegarACobro(alumnoId);
        }
        PrimaryContainer.Navigate(page);
    }

    private void ReportesFinanzas_Click(object sender, RoutedEventArgs e)
    {
        var page = App.Services.GetRequiredService<ReportesFinanzas>();
        if (page.DataContext is ReportesFinanzasViewModel vm)
        {
            vm.GenerarReporteCommand.Execute(null); // Refresca los datos en tiempo real desde SQLite
            // Subscribe to event exactly once to avoid memory leaks if clicked multiple times
            vm.SolicitaIrAExpediente -= NavegarAExpediente; 
            vm.SolicitaIrAExpediente += NavegarAExpediente;
        }
        PrimaryContainer.Navigate(page);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        PrimaryContainer.Navigate(App.Services.GetRequiredService<Configuracion>());
    }
}
