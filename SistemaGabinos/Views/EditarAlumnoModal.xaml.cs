using System.Windows;
using SistemaGabinos.ViewModels;

namespace SistemaGabinos.Views;

public partial class EditarAlumnoModal : Window
{
    public EditarAlumnoViewModel ViewModel { get; }

    public EditarAlumnoModal(EditarAlumnoViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;

        ViewModel.GuardadoExitoso += () =>
        {
            DialogResult = true;
            Close();
        };
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
