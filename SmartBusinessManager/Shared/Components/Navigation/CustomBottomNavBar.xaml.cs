using System.Windows.Input;

namespace SmartBusinessManager.Shared.Components.Navigation;

public partial class CustomBottomNavBar : ContentView
{
    public static readonly BindableProperty ActiveTabProperty =
        BindableProperty.Create(
            nameof(ActiveTab), 
            typeof(string), 
            typeof(CustomBottomNavBar), 
            string.Empty,
            propertyChanged: (bindable, oldValue, newValue) => 
                ((CustomBottomNavBar)bindable).UpdateVisualStates());

    public static readonly BindableProperty InsightBadgeCountProperty =
        BindableProperty.Create(
            nameof(InsightBadgeCount), 
            typeof(int), 
            typeof(CustomBottomNavBar), 
            0,
            propertyChanged: (bindable, oldValue, newValue) => 
                ((CustomBottomNavBar)bindable).UpdateBadgeState());

    public static readonly BindableProperty TabChangedCommandProperty =
        BindableProperty.Create(
            nameof(TabChangedCommand), 
            typeof(ICommand), 
            typeof(CustomBottomNavBar), 
            null);

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    public int InsightBadgeCount
    {
        get => (int)GetValue(InsightBadgeCountProperty);
        set => SetValue(InsightBadgeCountProperty, value);
    }

    public ICommand TabChangedCommand
    {
        get => (ICommand)GetValue(TabChangedCommandProperty);
        set => SetValue(TabChangedCommandProperty, value);
    }

    public CustomBottomNavBar()
    {
        InitializeComponent();
        
        // Wire up initial display states
        UpdateVisualStates();
        UpdateBadgeState();
    }

    private void UpdateVisualStates()
    {
        // Guard against premature execution during component loading
        if (DashboardIcon == null || ClientsIcon == null || FinanceIcon == null || InsightsIcon == null)
            return;

        // Fetch colors from application resources safely
        Color primaryColor = Colors.Blue;
        Color primaryContainerColor = Colors.LightBlue;
        Color outlineColor = Colors.Gray;

        if (Application.Current != null)
        {
            if (Application.Current.Resources.TryGetValue("Primary", out var p) && p is Color c1) primaryColor = c1;
            if (Application.Current.Resources.TryGetValue("PrimaryContainer", out var p2) && p2 is Color c2) primaryContainerColor = c2;
            if (Application.Current.Resources.TryGetValue("Outline", out var p3) && p3 is Color c3) outlineColor = c3;
        }

        // Reset all tabs to inactive visual style
        ResetTab(DashboardIcon, DashboardText, DashboardIndicator, outlineColor);
        ResetTab(ClientsIcon, ClientsText, ClientsIndicator, outlineColor);
        ResetTab(FinanceIcon, FinanceText, FinanceIndicator, outlineColor);
        ResetTab(InsightsIcon, InsightsText, InsightsIndicator, outlineColor);

        // Highlight the matching active tab using strict DESIGN.md tokens
        switch (ActiveTab?.ToLowerInvariant())
        {
            case "dashboard":
                HighlightTab(DashboardIcon, DashboardText, DashboardIndicator, primaryColor);
                break;
            case "clients":
                HighlightTab(ClientsIcon, ClientsText, ClientsIndicator, primaryColor);
                break;
            case "payments":
            case "finance":
                HighlightTab(FinanceIcon, FinanceText, FinanceIndicator, primaryColor);
                break;
            case "insights":
            case "ai":
                HighlightTab(InsightsIcon, InsightsText, InsightsIndicator, primaryColor); // Solid primary color highlighting
                break;
        }
    }

    private void ResetTab(Label icon, Label text, BoxView indicator, Color color)
    {
        icon.TextColor = color;
        text.TextColor = color;
        indicator.IsVisible = false;
    }

    private void HighlightTab(Label icon, Label text, BoxView indicator, Color color)
    {
        icon.TextColor = color;
        text.TextColor = color;
        indicator.IsVisible = true;
    }

    private void UpdateBadgeState()
    {
        if (BadgeBorder == null) return;
        
        // Show AI insights badge only if the count > 0
        BadgeBorder.IsVisible = InsightBadgeCount > 0;
    }

    private async void OnTabTapped(object sender, TappedEventArgs e)
    {
        var targetTab = e.Parameter as string;
        if (string.IsNullOrEmpty(targetTab)) return;

        // Bubble up standard shell route change if no custom command override is registered
        if (TabChangedCommand != null && TabChangedCommand.CanExecute(targetTab))
        {
            TabChangedCommand.Execute(targetTab);
        }
        else
        {
            // Direct programmatical shell route change - works instantly and is completely DRY
            // Routes: dashboard, clients, payments, insights
            await Shell.Current.GoToAsync($"///{targetTab}");
        }
    }
}
