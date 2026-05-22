using System.Windows.Input;

namespace SmartBusinessManager.Shared.Components.Cards;

public partial class AiInsightCard : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(AiInsightCard), string.Empty);

    public static readonly BindableProperty InsightContentProperty =
        BindableProperty.Create(nameof(InsightContent), typeof(string), typeof(AiInsightCard), string.Empty);

    public static readonly BindableProperty TimestampTextProperty =
        BindableProperty.Create(nameof(TimestampText), typeof(string), typeof(AiInsightCard), string.Empty,
            propertyChanged: (bindable, oldValue, newValue) => 
                ((AiInsightCard)bindable).UpdateTimestampVisibility());

    public static readonly BindableProperty ActionTextProperty =
        BindableProperty.Create(nameof(ActionText), typeof(string), typeof(AiInsightCard), string.Empty,
            propertyChanged: (bindable, oldValue, newValue) => 
                ((AiInsightCard)bindable).UpdateActionRowVisibility());

    public static readonly BindableProperty ActionCommandProperty =
        BindableProperty.Create(nameof(ActionCommand), typeof(ICommand), typeof(AiInsightCard), null);

    public static readonly BindableProperty ActionCommandParameterProperty =
        BindableProperty.Create(nameof(ActionCommandParameter), typeof(object), typeof(AiInsightCard), null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string InsightContent
    {
        get => (string)GetValue(InsightContentProperty);
        set => SetValue(InsightContentProperty, value);
    }

    public string TimestampText
    {
        get => (string)GetValue(TimestampTextProperty);
        set => SetValue(TimestampTextProperty, value);
    }

    public string ActionText
    {
        get => (string)GetValue(ActionTextProperty);
        set => SetValue(ActionTextProperty, value);
    }

    public ICommand ActionCommand
    {
        get => (ICommand)GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    public object ActionCommandParameter
    {
        get => (object)GetValue(ActionCommandParameterProperty);
        set => SetValue(ActionCommandParameterProperty, value);
    }

    public AiInsightCard()
    {
        InitializeComponent();
        UpdateTimestampVisibility();
        UpdateActionRowVisibility();
    }

    private void UpdateTimestampVisibility()
    {
        if (TimestampLabel != null)
        {
            TimestampLabel.IsVisible = !string.IsNullOrEmpty(TimestampText);
        }
    }

    private void UpdateActionRowVisibility()
    {
        if (ActionRow != null)
        {
            ActionRow.IsVisible = !string.IsNullOrEmpty(ActionText);
        }
    }

    private void OnActionTapped(object sender, EventArgs e)
    {
        if (ActionCommand != null && ActionCommand.CanExecute(ActionCommandParameter))
        {
            ActionCommand.Execute(ActionCommandParameter);
        }
    }
}
