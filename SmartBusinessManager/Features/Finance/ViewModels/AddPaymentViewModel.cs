using System.Collections.ObjectModel;
using SmartBusinessManager.Core.Models;
using SmartBusinessManager.Core.Services;
using SmartBusinessManager.Features.Clients.Models;
using SmartBusinessManager.Features.Finance.Models;

namespace SmartBusinessManager.Features.Finance.ViewModels;

public class AddPaymentViewModel : BaseViewModel
{
    private readonly ISupabaseService _supabaseService;

    private ObservableCollection<Client> _clients = new();
    public ObservableCollection<Client> Clients
    {
        get => _clients;
        set { _clients = value; OnPropertyChanged(); }
    }

    private Client _selectedClient;
    public Client SelectedClient
    {
        get => _selectedClient;
        set { _selectedClient = value; OnPropertyChanged(); }
    }

    private string _amountText = string.Empty;
    public string AmountText
    {
        get => _amountText;
        set { _amountText = value; OnPropertyChanged(); }
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }

    private DateTime _dueDate = DateTime.Today.AddDays(7);
    public DateTime DueDate
    {
        get => _dueDate;
        set { _dueDate = value; OnPropertyChanged(); }
    }

    private string _referenceNumber = string.Empty;
    public string ReferenceNumber
    {
        get => _referenceNumber;
        set { _referenceNumber = value; OnPropertyChanged(); }
    }

    private string _validationError;
    public string ValidationError
    {
        get => _validationError;
        set { _validationError = value; OnPropertyChanged(); }
    }

    public Command LoadClientsCommand { get; }
    public Command SaveCommand { get; }
    public Command CancelCommand { get; }

    public AddPaymentViewModel(ISupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        Title = "Add Payment";

        LoadClientsCommand = new Command(async () => await LoadClientsAsync());
        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await CancelAsync());

        // Proactively load clients list
        MainThread.BeginInvokeOnMainThread(async () => await LoadClientsAsync());
    }

    public async Task LoadClientsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ValidationError = null;

        try
        {
            var list = await _supabaseService.GetClientsAsync();
            // Filter to show active/lead clients for picker preferably
            Clients = new ObservableCollection<Client>(list.Where(c => !string.Equals(c.Status, "archived", StringComparison.OrdinalIgnoreCase)));
        }
        catch (Exception ex)
        {
            ValidationError = "Failed to load clients list.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (SelectedClient == null)
        {
            ValidationError = "Please select a client.";
            return;
        }

        if (string.IsNullOrWhiteSpace(AmountText) || !decimal.TryParse(AmountText, out var amount) || amount <= 0)
        {
            ValidationError = "Please enter a valid amount greater than 0.";
            return;
        }

        if (IsBusy) return;
        IsBusy = true;
        ValidationError = null;
        ErrorMessage = null;

        try
        {
            var payment = new Payment
            {
                ClientId = SelectedClient.Id,
                Amount = amount,
                Currency = "USD", // Fixed to USD for MVP
                Status = "pending",
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                DueDate = DateOnly.FromDateTime(DueDate),
                ReferenceNumber = string.IsNullOrWhiteSpace(ReferenceNumber) ? null : ReferenceNumber.Trim()
            };

            await _supabaseService.AddPaymentAsync(payment);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to log payment invoice. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
