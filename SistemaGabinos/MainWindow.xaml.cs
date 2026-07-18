using Microsoft.Extensions.DependencyInjection;
using SistemaGabinos.Views;
using System.Windows;

namespace SistemaGabinos;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += (s, e) => PrimaryContainer.Navigate(
            App.Services.GetRequiredService<PanelDeControl>()
        );
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
