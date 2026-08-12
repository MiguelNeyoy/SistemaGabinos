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

    private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
