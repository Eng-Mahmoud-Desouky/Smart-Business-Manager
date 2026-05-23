using System.Collections.ObjectModel;
using SmartBusinessManager.Core.Models;
using SmartBusinessManager.Core.Services;
using SmartBusinessManager.Features.Clients.Models;
using SmartBusinessManager.Features.Finance.Models;

namespace SmartBusinessManager.Features.Clients.ViewModels;

[QueryProperty(nameof(ClientId), "ClientId")]
public class ClientDetailViewModel : BaseViewModel
{
    private readonly ISupabaseService _supabaseService;

    private string _clientId;
    public string ClientId
    {
        get => _clientId;
        set
        {
            _clientId = value;
            OnPropertyChanged();
            if (!string.IsNullOrEmpty(_clientId))
            {
                MainThread.BeginInvokeOnMainThread(async () => await LoadDetailsAsync());
            }
        }
    }

    private Client _client;
    public Client Client
    {
        get => _client;
        set { _client = value; OnPropertyChanged(); }
    }

    private ObservableCollection<Interaction> _interactions = new();
    public ObservableCollection<Interaction> Interactions
    {
        get => _interactions;
        set { _interactions = value; OnPropertyChanged(); }
    }

    private ObservableCollection<Payment> _payments = new();
    public ObservableCollection<Payment> Payments
    {
        get => _payments;
        set { _payments = value; OnPropertyChanged(); }
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

    private bool _isInteractionsTabSelected = true;
    public bool IsInteractionsTabSelected
    {
        get => _isInteractionsTabSelected;
        set
        {
            _isInteractionsTabSelected = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsPaymentsTabSelected));
        }
    }

    public bool IsPaymentsTabSelected => !IsInteractionsTabSelected;

    private bool _isLogInteractionFormVisible;
    public bool IsLogInteractionFormVisible
    {
        get => _isLogInteractionFormVisible;
        set { _isLogInteractionFormVisible = value; OnPropertyChanged(); }
    }

    // ── Log Interaction Fields ─────────────────────────
    private string _interactionType = "Call";
    public string InteractionType
    {
        get => _interactionType;
        set { _interactionType = value; OnPropertyChanged(); }
    }

    private string _interactionSubject = string.Empty;
    public string InteractionSubject
    {
        get => _interactionSubject;
        set { _interactionSubject = value; OnPropertyChanged(); }
    }

    private string _interactionNotes = string.Empty;
    public string InteractionNotes
    {
        get => _interactionNotes;
        set { _interactionNotes = value; OnPropertyChanged(); }
    }

    private string _logValidationError;
    public string LogValidationError
    {
        get => _logValidationError;
        set { _logValidationError = value; OnPropertyChanged(); }
    }

    public List<string> InteractionTypes { get; } = new() { "Call", "Meeting", "Message", "Note" };

    public Command LoadDetailsCommand { get; }
    public Command LogInteractionCommand { get; }
    public Command ToggleTabCommand { get; }
    public Command ToggleLogFormCommand { get; }
    public Command<string> UpdateClientStatusCommand { get; }
    public Command DeleteClientCommand { get; }

    public ClientDetailViewModel(ISupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        Title = "Client Details";

        LoadDetailsCommand = new Command(async () => await LoadDetailsAsync());
        LogInteractionCommand = new Command(async () => await LogInteractionAsync());
        ToggleTabCommand = new Command<string>((tab) => IsInteractionsTabSelected = string.Equals(tab, "interactions", StringComparison.OrdinalIgnoreCase));
        ToggleLogFormCommand = new Command(() => IsLogInteractionFormVisible = !IsLogInteractionFormVisible);
        UpdateClientStatusCommand = new Command<string>(async (status) => await UpdateClientStatusAsync(status));
        DeleteClientCommand = new Command(async () => await DeleteClientAsync());
    }

    public async Task LoadDetailsAsync()
    {
        if (string.IsNullOrEmpty(ClientId)) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            // Load client details
            var clientModel = await _supabaseService.GetClientByIdAsync(ClientId);
            if (clientModel == null)
            {
                ErrorMessage = "Client not found.";
                return;
            }
            Client = clientModel;
            Title = Client.Name;

            // Load interactions timeline
            var interactionsList = await _supabaseService.GetInteractionsByClientAsync(ClientId);
            Interactions = new ObservableCollection<Interaction>(interactionsList);

            // Load payments
            var paymentsList = await _supabaseService.GetPaymentsByClientAsync(ClientId);
            Payments = new ObservableCollection<Payment>(paymentsList);

            // Compute metrics
            CalculateFinancials();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load details. Please pull to refresh.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CalculateFinancials()
    {
        decimal paid = 0;
        decimal pending = 0;

        foreach (var p in Payments)
        {
            if (string.Equals(p.Status, "paid", StringComparison.OrdinalIgnoreCase))
            {
                paid += p.Amount;
            }
            else if (string.Equals(p.Status, "pending", StringComparison.OrdinalIgnoreCase) || 
                     string.Equals(p.Status, "overdue", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(p.Status, "partial", StringComparison.OrdinalIgnoreCase))
            {
                pending += p.Amount;
            }
        }

        TotalRevenue = paid;
        PendingRevenue = pending;
    }

    private async Task LogInteractionAsync()
    {
        if (string.IsNullOrWhiteSpace(InteractionSubject))
        {
            LogValidationError = "Subject is required.";
            return;
        }

        IsBusy = true;
        LogValidationError = null;

        try
        {
            var interaction = new Interaction
            {
                ClientId = ClientId,
                Type = InteractionType.ToLowerInvariant(),
                Subject = InteractionSubject.Trim(),
                Notes = string.IsNullOrWhiteSpace(InteractionNotes) ? null : InteractionNotes.Trim(),
                InteractedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _supabaseService.AddInteractionAsync(interaction);

            // Update client's last_contacted_at in active object & DB
            Client.LastContactedAt = DateTime.UtcNow;
            await _supabaseService.UpdateClientAsync(Client);

            // Refresh details
            await LoadDetailsAsync();

            // Reset fields & close
            InteractionSubject = string.Empty;
            InteractionNotes = string.Empty;
            IsLogInteractionFormVisible = false;
        }
        catch (Exception ex)
        {
            LogValidationError = "Failed to log interaction. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UpdateClientStatusAsync(string newStatus)
    {
        if (Client == null || string.IsNullOrEmpty(newStatus)) return;

        IsBusy = true;
        try
        {
            Client.Status = newStatus.ToLowerInvariant();
            await _supabaseService.UpdateClientAsync(Client);
            OnPropertyChanged(nameof(Client));
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to update client status.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteClientAsync()
    {
        if (Client == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Delete Client", $"Are you sure you want to delete {Client.Name}?", "Yes", "No");
        if (!confirm) return;

        IsBusy = true;
        try
        {
            await _supabaseService.DeleteClientAsync(Client.Id);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to delete client. Make sure they have no associated payments or interactions.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
