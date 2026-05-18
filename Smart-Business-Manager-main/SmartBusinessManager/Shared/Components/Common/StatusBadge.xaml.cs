namespace SmartBusinessManager.Shared.Components.Common;

public partial class StatusBadge : ContentView
{
    public static readonly BindableProperty StatusProperty =
        BindableProperty.Create(
            nameof(Status), 
            typeof(string), 
            typeof(StatusBadge), 
            string.Empty,
            propertyChanged: (bindable, oldValue, newValue) => 
                ((StatusBadge)bindable).UpdateBadgeVisuals());

    public static readonly BindableProperty CustomTextProperty =
        BindableProperty.Create(
            nameof(CustomText), 
            typeof(string), 
            typeof(StatusBadge), 
            string.Empty,
            propertyChanged: (bindable, oldValue, newValue) => 
                ((StatusBadge)bindable).UpdateBadgeVisuals());

    public string Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string CustomText
    {
        get => (string)GetValue(CustomTextProperty);
        set => SetValue(CustomTextProperty, value);
    }

    public StatusBadge()
    {
        InitializeComponent();
        UpdateBadgeVisuals();
    }

    private void UpdateBadgeVisuals()
    {
        if (BadgeBorder == null || BadgeLabel == null) return;

        var statusKey = Status?.Trim().ToLowerInvariant() ?? "inactive";

        // 1. Establish the text representation
        var displayText = !string.IsNullOrEmpty(CustomText) 
            ? CustomText 
            : System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(statusKey);

        BadgeLabel.Text = displayText;

        // 2. Resolve themes and colors based on semantic design tokens in DESIGN.md
        Color backgroundColor;
        Color textColor;

        switch (statusKey)
        {
            case "active":
            case "paid":
            case "success":
            case "completed":
                backgroundColor = Color.FromArgb("#DCEEC9"); // SuccessContainer
                textColor = Color.FromArgb("#141F07");       // OnSuccessContainer
                break;

            case "lead":
            case "pending":
            case "warning":
                backgroundColor = Color.FromArgb("#FFEEC2"); // Warning / Secondary Container
                textColor = Color.FromArgb("#5F3D00");       // OnWarning / Secondary Text
                break;

            case "overdue":
            case "error":
            case "failed":
            case "cancelled":
                backgroundColor = Color.FromArgb("#FFDAD6"); // ErrorContainer
                textColor = Color.FromArgb("#410002");       // OnErrorContainer
                break;

            case "inactive":
            case "draft":
            default:
                backgroundColor = Color.FromArgb("#ECE9F2"); // SurfaceDim / Neutral Soft
                textColor = Color.FromArgb("#777587");       // Outline / Muted Gray
                break;
        }

        BadgeBorder.BackgroundColor = backgroundColor;
        BadgeLabel.TextColor = textColor;
    }
}
