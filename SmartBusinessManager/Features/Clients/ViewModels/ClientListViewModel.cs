using System.Collections.ObjectModel;
using SmartBusinessManager.Core.Models;
using SmartBusinessManager.Core.Services;
using SmartBusinessManager.Features.Clients.Models;

namespace SmartBusinessManager.Features.Clients.ViewModels;

public class ClientListViewModel : BaseViewModel
{
    private readonly ISupabaseService _supabaseService;

    private ObservableCollection<Client> _clients = new();
    public ObservableCollection<Client> Clients
    {
        get => _clients;
        set { _clients = value; OnPropertyChanged(); }
    }

    private ObservableCollection<Client> _filteredClients = new();
    public ObservableCollection<Client> FilteredClients
    {
        get => _filteredClients;
        set { _filteredClients = value; OnPropertyChanged(); }
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            ApplyFilters();
        }
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

    private bool _isEmpty;
    public bool IsEmpty
    {
        get => _isEmpty;
        set { _isEmpty = value; OnPropertyChanged(); }
    }

    public Command LoadClientsCommand { get; }
    public Command NavigateToAddClientCommand { get; }
    public Command<Client> NavigateToDetailCommand { get; }
    public Command<string> SelectFilterCommand { get; }

    public ClientListViewModel(ISupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
        Title = "Clients";

        LoadClientsCommand = new Command(async () => await LoadClientsAsync());
        NavigateToAddClientCommand = new Command(async () => await NavigateToAddClientAsync());
        NavigateToDetailCommand = new Command<Client>(async (client) => await NavigateToDetailAsync(client));
        SelectFilterCommand = new Command<string>((filter) => SelectedStatusFilter = filter);
    }

    public async Task LoadClientsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var list = await _supabaseService.GetClientsAsync();
            Clients = new ObservableCollection<Client>(list);
            ApplyFilters();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load clients. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyFilters()
    {
        if (Clients == null) return;

        var query = Clients.AsEnumerable();

        // Apply status filter
        if (!string.Equals(SelectedStatusFilter, "All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(c => string.Equals(c.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase));
        }

        // Apply search text
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim().ToLowerInvariant();
            query = query.Where(c => 
                (c.Name != null && c.Name.ToLowerInvariant().Contains(search)) ||
                (c.Company != null && c.Company.ToLowerInvariant().Contains(search)) ||
                (c.Email != null && c.Email.ToLowerInvariant().Contains(search))
            );
        }

        FilteredClients = new ObservableCollection<Client>(query);
        IsEmpty = FilteredClients.Count == 0;
    }

    private async Task NavigateToAddClientAsync()
    {
        await Shell.Current.GoToAsync("clients/add");
    }

    private async Task NavigateToDetailAsync(Client client)
    {
        if (client == null) return;
        
        await Shell.Current.GoToAsync("clients/detail", new Dictionary<string, object>
        {
            { "ClientId", client.Id }
        });
    }
}
