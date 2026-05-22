using System.Collections.ObjectModel;
using SmartBusinessManager.Core.Models;
using SmartBusinessManager.Core.Services;
using SmartBusinessManager.Features.AI.Models;
using SmartBusinessManager.Core.Helpers;

namespace SmartBusinessManager.Features.AI.ViewModels;

public class InsightsViewModel : BaseViewModel
{
    private readonly ISupabaseService _supabaseService;
    private readonly IAiService _aiService;
    private readonly SessionManager _sessionManager;

    private ObservableCollection<AiInsight> _insights = new();
    public ObservableCollection<AiInsight> Insights
    {
        get => _insights;
        set { _insights = value; OnPropertyChanged(); }
    }

    private ObservableCollection<AiInsight> _filteredInsights = new();
    public ObservableCollection<AiInsight> FilteredInsights
    {
        get => _filteredInsights;
        set { _filteredInsights = value; OnPropertyChanged(); }
    }

    private string _selectedFilter = "All";
    public string SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            _selectedFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private bool _isEmpty;
    public bool IsEmpty
    {
        get => _isEmpty;
        set { _isEmpty = value; OnPropertyChanged(); }
    }

    public Command LoadInsightsCommand { get; }
    public Command<string> SelectFilterCommand { get; }
    public Command<AiInsight> DismissInsightCommand { get; }
    public Command<AiInsight> PerformActionCommand { get; }

    public InsightsViewModel(ISupabaseService supabaseService, IAiService aiService, SessionManager sessionManager)
    {
        _supabaseService = supabaseService;
        _aiService = aiService;
        _sessionManager = sessionManager;
        Title = "AI Insights";

        LoadInsightsCommand = new Command(async () => await LoadInsightsAsync());
        SelectFilterCommand = new Command<string>((filter) => SelectedFilter = filter);
        DismissInsightCommand = new Command<AiInsight>(async (insight) => await DismissInsightAsync(insight));
        PerformActionCommand = new Command<AiInsight>(async (insight) => await PerformActionAsync(insight));
    }

    public async Task LoadInsightsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            // 1. Generate new insights in DB using Postgres function
            string userId = _sessionManager.CurrentUserId;
            if (!string.IsNullOrEmpty(userId))
            {
                await _aiService.GenerateInsightsAsync(userId);
            }

            // 2. Fetch insights
            var list = await _supabaseService.GetInsightsAsync();

            // Filter out read ones
            var activeInsights = list.Where(i => !i.IsRead).ToList();
            Insights = new ObservableCollection<AiInsight>(activeInsights);

            ApplyFilters();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load smart insights. Pull to refresh.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyFilters()
    {
        if (Insights == null) return;

        var query = Insights.AsEnumerable();

        if (string.Equals(SelectedFilter, "High Priority", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(i => i.Priority <= 2);
        }
        else if (string.Equals(SelectedFilter, "Payments", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(i => string.Equals(i.InsightType, "overdue_payment", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(i.InsightType, "payment_due_soon", StringComparison.OrdinalIgnoreCase));
        }
        else if (string.Equals(SelectedFilter, "Activity", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(i => string.Equals(i.InsightType, "follow_up_needed", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(i.InsightType, "inactive_client", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(i.InsightType, "high_value_client", StringComparison.OrdinalIgnoreCase));
        }

        FilteredInsights = new ObservableCollection<AiInsight>(query);
        IsEmpty = FilteredInsights.Count == 0;
    }

    private async Task DismissInsightAsync(AiInsight insight)
    {
        if (insight == null) return;

        try
        {
            await _supabaseService.MarkInsightAsReadAsync(insight.Id);
            Insights.Remove(insight);
            ApplyFilters();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to dismiss insight.";
        }
    }

    private async Task PerformActionAsync(AiInsight insight)
    {
        if (insight == null || string.IsNullOrEmpty(insight.ClientId)) return;

        // Navigate directly to the client's detail page where the action should occur
        await Shell.Current.GoToAsync($"clients/detail?ClientId={insight.ClientId}");
    }
}
