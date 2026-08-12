using System.Windows;
using System.Windows.Controls;

namespace SistemaGabinos.Controls;

public partial class CardSection : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(CardSection), new PropertyMetadata(""));
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public CardSection()
    {
        InitializeComponent();
    }
}
