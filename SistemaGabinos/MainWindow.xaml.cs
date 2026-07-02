using SistemaGabinos.Views;
using System.Windows;

namespace SistemaGabinos
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FunctionPanel_Click(object sender, RoutedEventArgs e)
        {
            PrimaryContainer.Navigate(new PanelDeControl());
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
