using System.Windows;
using System.Windows.Controls;

namespace SistemaGabinos.Controls;

public partial class ToggleGroup : UserControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(ToggleGroup), new PropertyMetadata(""));
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty Option1TextProperty =
        DependencyProperty.Register(nameof(Option1Text), typeof(string), typeof(ToggleGroup), new PropertyMetadata(""));
    public string Option1Text
    {
        get => (string)GetValue(Option1TextProperty);
        set => SetValue(Option1TextProperty, value);
    }

    public static readonly DependencyProperty Option1CheckedProperty =
        DependencyProperty.Register(nameof(Option1Checked), typeof(bool), typeof(ToggleGroup), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public bool Option1Checked
    {
        get => (bool)GetValue(Option1CheckedProperty);
        set => SetValue(Option1CheckedProperty, value);
    }

    public static readonly DependencyProperty Option2TextProperty =
        DependencyProperty.Register(nameof(Option2Text), typeof(string), typeof(ToggleGroup), new PropertyMetadata(""));
    public string Option2Text
    {
        get => (string)GetValue(Option2TextProperty);
        set => SetValue(Option2TextProperty, value);
    }

    public static readonly DependencyProperty Option2CheckedProperty =
        DependencyProperty.Register(nameof(Option2Checked), typeof(bool), typeof(ToggleGroup), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public bool Option2Checked
    {
        get => (bool)GetValue(Option2CheckedProperty);
        set => SetValue(Option2CheckedProperty, value);
    }

    public ToggleGroup()
    {
        InitializeComponent();
    }
}
