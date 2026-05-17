namespace SmartBusinessManager.Shared.Components.Forms;

public partial class FormInputField : ContentView
{
    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FormInputField), string.Empty);

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text), 
            typeof(string), 
            typeof(FormInputField), 
            string.Empty,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FormInputField), string.Empty);

    public static readonly BindableProperty KeyboardProperty =
        BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(FormInputField), Keyboard.Default);

    public static readonly BindableProperty IsPasswordProperty =
        BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FormInputField), false,
            propertyChanged: (bindable, oldValue, newValue) => 
            {
                var control = (FormInputField)bindable;
                if ((bool)newValue)
                {
                    control.IsPasswordControl = true;
                }
            });

    public static readonly BindableProperty IsPasswordControlProperty =
        BindableProperty.Create(nameof(IsPasswordControl), typeof(bool), typeof(FormInputField), false);

    public static readonly BindableProperty HasErrorProperty =
        BindableProperty.Create(nameof(HasError), typeof(bool), typeof(FormInputField), false);

    public static readonly BindableProperty ErrorMessageProperty =
        BindableProperty.Create(nameof(ErrorMessage), typeof(string), typeof(FormInputField), string.Empty);

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public bool IsPasswordControl
    {
        get => (bool)GetValue(IsPasswordControlProperty);
        set => SetValue(IsPasswordControlProperty, value);
    }

    public bool HasError
    {
        get => (bool)GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public FormInputField()
    {
        InitializeComponent();
    }

    private void OnEntryFocused(object sender, FocusEventArgs e)
    {
        // Don't overwrite error state visuals
        if (HasError) return;

        var primaryColor = (Color)Application.Current.Resources["Primary"];
        InputBorder.Stroke = primaryColor;
        InputBorder.StrokeThickness = 1.5;
    }

    private void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        // Don't overwrite error state visuals
        if (HasError) return;

        var surfaceDimColor = (Color)Application.Current.Resources["SurfaceDim"];
        InputBorder.Stroke = surfaceDimColor;
        InputBorder.StrokeThickness = 1;
    }

    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        // Toggle entry password emulation state
        CoreEntry.IsPassword = !CoreEntry.IsPassword;

        // Toggle representation character glyph
        TogglePasswordButton.Text = CoreEntry.IsPassword ? "👁" : "🙈";
    }
}
