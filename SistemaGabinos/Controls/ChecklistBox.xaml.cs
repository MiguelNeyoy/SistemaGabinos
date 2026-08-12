using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace SistemaGabinos.Controls;

public partial class ChecklistBox : UserControl
{
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ChecklistBox));
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty TotalProperty =
        DependencyProperty.Register(nameof(Total), typeof(decimal), typeof(ChecklistBox), new FrameworkPropertyMetadata(0m, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public decimal Total
    {
        get => (decimal)GetValue(TotalProperty);
        set => SetValue(TotalProperty, value);
    }

    public ChecklistBox()
    {
        InitializeComponent();
    }
}
