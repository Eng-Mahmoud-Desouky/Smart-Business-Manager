using System.Windows.Input;

namespace SmartBusinessManager.Shared.Components.Cards;

public partial class ClientSummaryCard : ContentView
{
    public static readonly BindableProperty ClientNameProperty =
        BindableProperty.Create(
            nameof(ClientName), 
            typeof(string), 
            typeof(ClientSummaryCard), 
            string.Empty,
            propertyChanged: OnClientNameChanged);

    public static readonly BindableProperty CompanyProperty =
        BindableProperty.Create(nameof(Company), typeof(string), typeof(ClientSummaryCard), string.Empty);

    public static readonly BindableProperty EmailProperty =
        BindableProperty.Create(nameof(Email), typeof(string), typeof(ClientSummaryCard), string.Empty);

    public static readonly BindableProperty StatusProperty =
        BindableProperty.Create(nameof(Status), typeof(string), typeof(ClientSummaryCard), string.Empty);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(ClientSummaryCard), null);

    public static readonly BindableProperty TapCommandParameterProperty =
        BindableProperty.Create(nameof(TapCommandParameter), typeof(object), typeof(ClientSummaryCard), null);

    public string ClientName
    {
        get => (string)GetValue(ClientNameProperty);
        set => SetValue(ClientNameProperty, value);
    }

    public string Company
    {
        get => (string)GetValue(CompanyProperty);
        set => SetValue(CompanyProperty, value);
    }

    public string Email
    {
        get => (string)GetValue(EmailProperty);
        set => SetValue(EmailProperty, value);
    }

    public string Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public object TapCommandParameter
    {
        get => (object)GetValue(TapCommandParameterProperty);
        set => SetValue(TapCommandParameterProperty, value);
    }

    public ClientSummaryCard()
    {
        InitializeComponent();
        UpdateAvatarText();
    }

    private static void OnClientNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((ClientSummaryCard)bindable).UpdateAvatarText();
    }

    private void UpdateAvatarText()
    {
        if (AvatarLabel == null) return;

        var name = ClientName?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name))
        {
            AvatarLabel.Text = "??";
            return;
        }

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            AvatarLabel.Text = parts[0][0].ToString().ToUpperInvariant();
        }
        else if (parts.Length >= 2)
        {
            AvatarLabel.Text = (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpperInvariant();
        }
    }

    private void OnCardTapped(object sender, EventArgs e)
    {
        if (TapCommand != null && TapCommand.CanExecute(TapCommandParameter))
        {
            TapCommand.Execute(TapCommandParameter);
        }
    }
}
