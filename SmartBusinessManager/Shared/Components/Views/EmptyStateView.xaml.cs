using System.Windows.Input;

namespace SmartBusinessManager.Shared.Components.Views;

public partial class EmptyStateView : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title), 
            typeof(string), 
            typeof(EmptyStateView), 
            "No Data Available");

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description), 
            typeof(string), 
            typeof(EmptyStateView), 
            "Try adding a new entry to get started.");

    public static readonly BindableProperty GlyphProperty =
        BindableProperty.Create(
            nameof(Glyph), 
            typeof(string), 
            typeof(EmptyStateView), 
            "📁");

    public static readonly BindableProperty ActionButtonTextProperty =
        BindableProperty.Create(
            nameof(ActionButtonText), 
            typeof(string), 
            typeof(EmptyStateView), 
            string.Empty);

    public static readonly BindableProperty ActionButtonCommandProperty =
        BindableProperty.Create(
            nameof(ActionButtonCommand), 
            typeof(ICommand), 
            typeof(EmptyStateView), 
            null);

    public static readonly BindableProperty ActionButtonCommandParameterProperty =
        BindableProperty.Create(
            nameof(ActionButtonCommandParameter), 
            typeof(object), 
            typeof(EmptyStateView), 
            null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public string ActionButtonText
    {
        get => (string)GetValue(ActionButtonTextProperty);
        set => SetValue(ActionButtonTextProperty, value);
    }

    public ICommand ActionButtonCommand
    {
        get => (ICommand)GetValue(ActionButtonCommandProperty);
        set => SetValue(ActionButtonCommandProperty, value);
    }

    public object ActionButtonCommandParameter
    {
        get => (object)GetValue(ActionButtonCommandParameterProperty);
        set => SetValue(ActionButtonCommandParameterProperty, value);
    }

    public EmptyStateView()
    {
        InitializeComponent();
    }
}
