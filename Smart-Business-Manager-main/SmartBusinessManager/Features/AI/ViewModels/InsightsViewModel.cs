using System.Collections.ObjectModel;
using System.Windows.Input;
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

    private ObservableCollection<AiInsight> _insights;
    public ObservableCollection<AiInsight> Insights
    {
        get => _insights;
        set { _insights = value; OnPropertyChanged(); }
    }

    private bool _isEmpty;
    public bool IsEmpty
    {
        get => _isEmpty;
        set { _isEmpty = value; OnPropertyChanged(); }
    }

    private bool _hasInsights;
    public bool HasInsights
    {
        get => _hasInsights;
        set { _hasInsights = value; OnPropertyChanged(); }
    }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        set { _hasError = value; OnPropertyChanged(); }
    }

    public ICommand LoadInsightsCommand { get; }
    public ICommand MarkAsReadCommand { get; }
    public ICommand GenerateInsightsCommand { get; }

    public InsightsViewModel(
        ISupabaseService supabaseService,
        IAiService aiService,
        SessionManager sessionManager)
    {
        _supabaseService = supabaseService;
        _aiService = aiService;
        _sessionManager = sessionManager;
        Title = "التحليلات الذكية";

        Insights = new ObservableCollection<AiInsight>();
        IsEmpty = true;
        HasInsights = false;
        HasError = false;

        LoadInsightsCommand = new Command(async () => await LoadInsightsAsync());
        MarkAsReadCommand = new Command<AiInsight>(async (insight) => await MarkAsReadAsync(insight));
        GenerateInsightsCommand = new Command(async () => await GenerateInsightsAsync());
    }

    private async Task LoadInsightsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        HasError = false;

        try
        {
            var list = await _supabaseService.GetInsightsAsync();
            Insights.Clear();
            foreach (var item in list)
            {
                Insights.Add(item);
            }
            IsEmpty = Insights.Count == 0;
            HasInsights = Insights.Count > 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = "عذراً، فشل تحميل التحليلات الذكية. يرجى المحاولة لاحقاً.";
            HasError = true;
            System.Diagnostics.Debug.WriteLine($"Error in LoadInsightsAsync: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task MarkAsReadAsync(AiInsight insight)
    {
        if (insight == null) return;

        try
        {
            await _supabaseService.MarkInsightAsReadAsync(insight.Id);
            Insights.Remove(insight);
            IsEmpty = Insights.Count == 0;
            HasInsights = Insights.Count > 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = "فشل تحديث حالة النصيحة. يرجى المحاولة لاحقاً.";
            HasError = true;
            System.Diagnostics.Debug.WriteLine($"Error in MarkAsReadAsync: {ex.Message}");
        }
    }

    private async Task GenerateInsightsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        HasError = false;

        try
        {
            var userId = _sessionManager.CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                ErrorMessage = "يجب تسجيل الدخول أولاً لتوليد التحليلات.";
                HasError = true;
                return;
            }

            var success = await _aiService.GenerateInsightsAsync(userId);
            if (success)
            {
                await LoadInsightsAsync();
            }
            else
            {
                ErrorMessage = "فشل توليد التحليلات الذكية بالذكاء الاصطناعي.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "حدث خطأ أثناء توليد التحليلات.";
            HasError = true;
            System.Diagnostics.Debug.WriteLine($"Error in GenerateInsightsAsync: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
