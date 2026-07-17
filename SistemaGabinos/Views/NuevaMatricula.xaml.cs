using SistemaGabinos.ViewModels;
using System.Windows.Controls;

namespace SistemaGabinos.Views;

public partial class NuevaMatricula : Page
{
    public NuevaMatricula(NuevaMatriculaViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
