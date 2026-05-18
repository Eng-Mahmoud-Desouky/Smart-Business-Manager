using System.Windows.Input;

namespace SmartBusinessManager.Shared.Components.Navigation;

public partial class SharedAppBar : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(SharedAppBar), string.Empty);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(SharedAppBar), string.Empty,
            propertyChanged: (bindable, oldValue, newValue) => 
                ((SharedAppBar)bindable).UpdateSubtitleVisibility());

    public static readonly BindableProperty ShowBackButtonProperty =
        BindableProperty.Create(nameof(ShowBackButton), typeof(bool), typeof(SharedAppBar), false);

    public static readonly BindableProperty HasAiThemeProperty =
        BindableProperty.Create(nameof(HasAiTheme), typeof(bool), typeof(SharedAppBar), false);

    public static readonly BindableProperty RightActionIconProperty =
        BindableProperty.Create(nameof(RightActionIcon), typeof(string), typeof(SharedAppBar), string.Empty,
            propertyChanged: (bindable, oldValue, newValue) => 
                ((SharedAppBar)bindable).UpdateActionIconVisibility());

    public static readonly BindableProperty RightActionCommandProperty =
        BindableProperty.Create(nameof(RightActionCommand), typeof(ICommand), typeof(SharedAppBar), null);

    public static readonly BindableProperty RightActionCommandParameterProperty =
        BindableProperty.Create(nameof(RightActionCommandParameter), typeof(object), typeof(SharedAppBar), null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    public bool HasAiTheme
    {
        get => (bool)GetValue(HasAiThemeProperty);
        set => SetValue(HasAiThemeProperty, value);
    }

    public string RightActionIcon
    {
        get => (string)GetValue(RightActionIconProperty);
        set => SetValue(RightActionIconProperty, value);
    }

    public ICommand RightActionCommand
    {
        get => (ICommand)GetValue(RightActionCommandProperty);
        set => SetValue(RightActionCommandProperty, value);
    }

    public object RightActionCommandParameter
    {
        get => (object)GetValue(RightActionCommandParameterProperty);
        set => SetValue(RightActionCommandParameterProperty, value);
    }

    public SharedAppBar()
    {
        InitializeComponent();
        UpdateSubtitleVisibility();
        UpdateActionIconVisibility();
    }

    private void UpdateSubtitleVisibility()
    {
        if (SubtitleLabel != null)
        {
            SubtitleLabel.IsVisible = !string.IsNullOrEmpty(Subtitle);
        }
    }

    private void UpdateActionIconVisibility()
    {
        if (ActionButton != null)
        {
            ActionButton.IsVisible = !string.IsNullOrEmpty(RightActionIcon);
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        // Native back-navigation bubble
        await Shell.Current.GoToAsync("..");
    }
}
