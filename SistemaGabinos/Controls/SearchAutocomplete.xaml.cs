using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SistemaGabinos.Controls;

public partial class SearchAutocomplete : UserControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(SearchAutocomplete), new PropertyMetadata(""));
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(SearchAutocomplete), new PropertyMetadata("Buscar..."));
    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(SearchAutocomplete));
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(SearchAutocomplete), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty SearchTextProperty =
        DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(SearchAutocomplete), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public static readonly DependencyProperty IsDropdownOpenProperty =
        DependencyProperty.Register(nameof(IsDropdownOpen), typeof(bool), typeof(SearchAutocomplete), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public bool IsDropdownOpen
    {
        get => (bool)GetValue(IsDropdownOpenProperty);
        set => SetValue(IsDropdownOpenProperty, value);
    }

    public event EventHandler? SearchAccepted;

    public SearchAutocomplete()
    {
        InitializeComponent();
    }

    private void OnSearchTextBoxKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SearchAccepted?.Invoke(this, EventArgs.Empty);
        }
    }
}
