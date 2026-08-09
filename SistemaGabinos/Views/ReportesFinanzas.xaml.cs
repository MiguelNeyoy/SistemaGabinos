using System.Windows.Controls;
using System.Windows.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.ViewModels;

namespace SistemaGabinos.Views;

public partial class ReportesFinanzas : Page
{
    public ReportesFinanzasViewModel ViewModel { get; }

    public ReportesFinanzas(ReportesFinanzasViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
    }

    private void DeudoresGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DeudoresGrid.SelectedItem is AlumnoDeudorDto alumnoSeleccionado)
        {
            if (ViewModel.IrAExpedienteCommand.CanExecute(alumnoSeleccionado))
            {
                ViewModel.IrAExpedienteCommand.Execute(alumnoSeleccionado);
            }
        }
    }
}
