using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SistemaGabinos.ViewModels;

namespace SistemaGabinos.Views;

public partial class VentanillaCobro : Page
{
    public CobroExpresViewModel ViewModel { get; }

    public VentanillaCobro()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<CobroExpresViewModel>();
        DataContext = ViewModel;
    }

    public void CargarDatosAlumno(int alumnoId, int? deudaIdInicial = null)
    {
        ViewModel.PrecargarDatos(alumnoId, deudaIdInicial);
    }
}
