using System.Windows.Input;

namespace SmartBusinessManager.Shared.Components.Cards;

public partial class PaymentRecordCard : ContentView
{
    public static readonly BindableProperty ClientNameProperty =
        BindableProperty.Create(nameof(ClientName), typeof(string), typeof(PaymentRecordCard), string.Empty);

    public static readonly BindableProperty AmountTextProperty =
        BindableProperty.Create(nameof(AmountText), typeof(string), typeof(PaymentRecordCard), string.Empty);

    public static readonly BindableProperty DateTextProperty =
        BindableProperty.Create(nameof(DateText), typeof(string), typeof(PaymentRecordCard), string.Empty);

    public static readonly BindableProperty StatusProperty =
        BindableProperty.Create(nameof(Status), typeof(string), typeof(PaymentRecordCard), string.Empty);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(PaymentRecordCard), null);

    public static readonly BindableProperty TapCommandParameterProperty =
        BindableProperty.Create(nameof(TapCommandParameter), typeof(object), typeof(PaymentRecordCard), null);

    public string ClientName
    {
        get => (string)GetValue(ClientNameProperty);
        set => SetValue(ClientNameProperty, value);
    }

    public string AmountText
    {
        get => (string)GetValue(AmountTextProperty);
        set => SetValue(AmountTextProperty, value);
    }

    public string DateText
    {
        get => (string)GetValue(DateTextProperty);
        set => SetValue(DateTextProperty, value);
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

    public PaymentRecordCard()
    {
        InitializeComponent();
    }

    private void OnCardTapped(object sender, EventArgs e)
    {
        if (TapCommand != null && TapCommand.CanExecute(TapCommandParameter))
        {
            TapCommand.Execute(TapCommandParameter);
        }
    }
}
