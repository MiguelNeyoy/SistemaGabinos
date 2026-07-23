using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaGabinos.ViewModels;

namespace SistemaGabinos.Views;

public partial class ExpedienteAlumno : Page
{
    public ExpedienteAlumnoViewModel ViewModel { get; }

    public ExpedienteAlumno()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ExpedienteAlumnoViewModel>();
        DataContext = ViewModel;
    }

    public ExpedienteAlumno(int alumnoId) : this()
    {
        ViewModel.CargarAlumno(alumnoId);
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }
}
