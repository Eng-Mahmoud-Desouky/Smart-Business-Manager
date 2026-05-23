using System.Collections.ObjectModel;
using SmartBusinessManager.Core.Models;
using SmartBusinessManager.Core.Services;
using SmartBusinessManager.Features.Finance.Models;
using SmartBusinessManager.Features.Finance.Views;

namespace SmartBusinessManager.Features.Finance.ViewModels;

public class PaymentListViewModel : BaseViewModel
{
    private readonly ISupabaseService _supabaseService;

    private ObservableCollection<Payment> _payments = new();
    public ObservableCollection<Payment> Payments
    {
        get => _payments;
        set { _payments = value; OnPropertyChanged(); }
    }

    private ObservableCollection<Payment> _filteredPayments = new();
    public ObservableCollection<Payment> FilteredPayments
    {
        get => _filteredPayments;
        set { _filteredPayments = value; OnPropertyChanged(); }
    }

    private string _selectedStatusFilter = "All";
    public string SelectedStatusFilter
    {
        get => _selectedStatusFilter;
        set
        {
            _selectedStatusFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private decimal _totalRevenue;
    public decimal TotalRevenue
    {
        get => _totalRevenue;
        set { _totalRevenue = value; OnPropertyChanged(); }
    }

    private decimal _pendingRevenue;
    public decimal PendingRevenue
    {
        get => _pendingRevenue;
        set { _pendingRevenue = value; OnPropertyChanged(); }
    }

    private int _overdueCount;
    public int OverdueCount
    {
        get => _overdueCount;
        set { _overdueCount = value; OnPropertyChanged(); }
    }

    private bool _isEmpty;
    public bool IsEmpty
    {
        get => _isEmpty;
        set { _isEmpty = value; OnPropertyChanged(); }
    }

    public Command LoadPaymentsCommand { get; }
    public Command NavigateToAddPaymentCommand { get; }
    public Command<string> SelectFilterCommand { get; }
    public Command<Payment> MarkAsPaidCommand { get; }

    public PaymentListViewModel(ISupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        Title = "Finance & Payments";

        LoadPaymentsCommand = new Command(async () => await LoadPaymentsAsync());
        NavigateToAddPaymentCommand = new Command(async () => await NavigateToAddPaymentAsync());
        SelectFilterCommand = new Command<string>((filter) => SelectedStatusFilter = filter);
        MarkAsPaidCommand = new Command<Payment>(async (payment) => await MarkAsPaidAsync(payment));
    }

    public async Task LoadPaymentsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            // 1. Fetch payments
            var paymentsList = await _supabaseService.GetPaymentsAsync();

            // 2. Fetch clients to map ClientName
            var clientsList = await _supabaseService.GetClientsAsync();
            var clientMap = clientsList.ToDictionary(c => c.Id, c => c.Name);

            foreach (var payment in paymentsList)
            {
                if (payment.ClientId != null && clientMap.TryGetValue(payment.ClientId, out var clientName))
                {
                    payment.ClientName = clientName;
                }
                else
                {
                    payment.ClientName = "Unknown Client";
                }
            }

            Payments = new ObservableCollection<Payment>(paymentsList);

            // 3. Compute metrics
            CalculateKPIs();

            // 4. Apply status filters
            ApplyFilters();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load payments. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CalculateKPIs()
    {
        decimal paid = 0;
        decimal pending = 0;
        int overdue = 0;

        foreach (var p in Payments)
        {
            if (string.Equals(p.Status, "paid", StringComparison.OrdinalIgnoreCase))
            {
                paid += p.Amount;
            }
            else if (string.Equals(p.Status, "pending", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(p.Status, "partial", StringComparison.OrdinalIgnoreCase))
            {
                pending += p.Amount;
            }
            else if (string.Equals(p.Status, "overdue", StringComparison.OrdinalIgnoreCase))
            {
                pending += p.Amount;
                overdue++;
            }
        }

        TotalRevenue = paid;
        PendingRevenue = pending;
        OverdueCount = overdue;
    }

    private void ApplyFilters()
    {
        if (Payments == null) return;

        var query = Payments.AsEnumerable();

        if (!string.Equals(SelectedStatusFilter, "All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(p => string.Equals(p.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase));
        }

        FilteredPayments = new ObservableCollection<Payment>(query);
        IsEmpty = FilteredPayments.Count == 0;
    }

    private async Task NavigateToAddPaymentAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddPaymentPage));
    }

    private async Task MarkAsPaidAsync(Payment payment)
    {
        if (payment == null || string.Equals(payment.Status, "paid", StringComparison.OrdinalIgnoreCase)) return;

        IsBusy = true;
        try
        {
            payment.Status = "paid";
            payment.PaidAt = DateTime.UtcNow;
            await _supabaseService.UpdatePaymentAsync(payment);

            // Recalculate KPIs and re-filter
            CalculateKPIs();
            ApplyFilters();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to update payment status.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
