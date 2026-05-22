using System.Collections.ObjectModel;
using SmartBusinessManager.Core.Models;
using SmartBusinessManager.Core.Services;
using SmartBusinessManager.Features.AI.Models;
using SmartBusinessManager.Features.Clients.Models;
using SmartBusinessManager.Features.Finance.Models;

namespace SmartBusinessManager.Features.Dashboard.ViewModels;

public class RecentActivityItem
{
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Icon { get; set; }
    public string IconColor { get; set; } // Hex string from DESIGN.md (e.g. Primary, Secondary, Warning, Danger)
    public DateTime Timestamp { get; set; }
    public string DisplayTime => Timestamp.ToLocalTime().ToString("MMM dd, h:mm tt");
    public string ClientId { get; set; }
}

public class DashboardViewModel : BaseViewModel
{
    private readonly ISupabaseService _supabaseService;

    private int _totalClientsCount;
    public int TotalClientsCount
    {
        get => _totalClientsCount;
        set { _totalClientsCount = value; OnPropertyChanged(); }
    }

    private string _monthlyRevenueText = "$0.00";
    public string MonthlyRevenueText
    {
        get => _monthlyRevenueText;
        set { _monthlyRevenueText = value; OnPropertyChanged(); }
    }

    private string _pendingPaymentsText = "$0.00";
    public string PendingPaymentsText
    {
        get => _pendingPaymentsText;
        set { _pendingPaymentsText = value; OnPropertyChanged(); }
    }

    private ObservableCollection<AiInsight> _topInsights = new();
    public ObservableCollection<AiInsight> TopInsights
    {
        get => _topInsights;
        set { _topInsights = value; OnPropertyChanged(); }
    }

    private ObservableCollection<RecentActivityItem> _recentActivities = new();
    public ObservableCollection<RecentActivityItem> RecentActivities
    {
        get => _recentActivities;
        set { _recentActivities = value; OnPropertyChanged(); }
    }

    public Command RefreshDashboardCommand { get; }
    public Command NavigateToClientsCommand { get; }
    public Command NavigateToFinanceCommand { get; }
    public Command NavigateToInsightsCommand { get; }
    public Command<RecentActivityItem> NavigateToClientDetailCommand { get; }

    public DashboardViewModel(ISupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        Title = "Dashboard";

        RefreshDashboardCommand = new Command(async () => await RefreshDashboardAsync());
        NavigateToClientsCommand = new Command(async () => await Shell.Current.GoToAsync("///clients"));
        NavigateToFinanceCommand = new Command(async () => await Shell.Current.GoToAsync("///payments"));
        NavigateToInsightsCommand = new Command(async () => await Shell.Current.GoToAsync("///insights"));
        NavigateToClientDetailCommand = new Command<RecentActivityItem>(async (item) => await NavigateToClientDetailAsync(item));
    }

    public async Task RefreshDashboardAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            // 1. Load Clients
            var clients = await _supabaseService.GetClientsAsync();
            TotalClientsCount = clients.Count;

            // 2. Load Payments
            var payments = await _supabaseService.GetPaymentsAsync();
            decimal monthlyPaid = 0;
            decimal pending = 0;
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            foreach (var p in payments)
            {
                if (string.Equals(p.Status, "paid", StringComparison.OrdinalIgnoreCase))
                {
                    if (p.PaidAt.HasValue && p.PaidAt.Value.Month == currentMonth && p.PaidAt.Value.Year == currentYear)
                    {
                        monthlyPaid += p.Amount;
                    }
                }
                else if (string.Equals(p.Status, "pending", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(p.Status, "overdue", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(p.Status, "partial", StringComparison.OrdinalIgnoreCase))
                {
                    pending += p.Amount;
                }
            }

            MonthlyRevenueText = monthlyPaid.ToString("C");
            PendingPaymentsText = pending.ToString("C");

            // 3. Load AI Insights (limit to top 2 high-priority)
            var insights = await _supabaseService.GetInsightsAsync();
            var unreadHighPriority = insights
                .Where(i => !i.IsRead && i.Priority <= 2)
                .OrderBy(i => i.Priority)
                .Take(2)
                .ToList();
            TopInsights = new ObservableCollection<AiInsight>(unreadHighPriority);

            // 4. Generate Recent Activity Feed (aggregate client registrations, payments, and interactions)
            var activities = new List<RecentActivityItem>();

            // Client registrations (last 5)
            foreach (var c in clients.Take(5))
            {
                activities.Add(new RecentActivityItem
                {
                    Title = "New Client Onboarded",
                    Subtitle = $"{c.Name} was added to CRM",
                    Icon = "👤",
                    IconColor = "#3525CD", // Primary Indigo
                    Timestamp = c.CreatedAt,
                    ClientId = c.Id
                });
            }

            // Payments (last 5)
            foreach (var p in payments.Take(5))
            {
                var client = clients.FirstOrDefault(c => c.Id == p.ClientId);
                var clientName = client?.Name ?? "Client";
                var isPaid = string.Equals(p.Status, "paid", StringComparison.OrdinalIgnoreCase);

                activities.Add(new RecentActivityItem
                {
                    Title = isPaid ? "Payment Received" : "Invoice Created",
                    Subtitle = isPaid ? $"Received {p.Amount:C} from {clientName}" : $"Invoiced {p.Amount:C} to {clientName}",
                    Icon = "💰",
                    IconColor = isPaid ? "#006C49" : "#A44100", // Success Emerald vs Warning Amber
                    Timestamp = isPaid ? (p.PaidAt ?? DateTime.UtcNow) : p.PaidAt ?? DateTime.UtcNow, // Falback timestamp
                    ClientId = p.ClientId
                });
            }

            // Order by timestamp desc and take 6
            var sortedActivities = activities
                .OrderByDescending(a => a.Timestamp)
                .Take(6)
                .ToList();

            RecentActivities = new ObservableCollection<RecentActivityItem>(sortedActivities);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load dashboard data. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task NavigateToClientDetailAsync(RecentActivityItem item)
    {
        if (item == null || string.IsNullOrEmpty(item.ClientId)) return;

        await Shell.Current.GoToAsync($"clients/detail?ClientId={item.ClientId}");
    }
}
