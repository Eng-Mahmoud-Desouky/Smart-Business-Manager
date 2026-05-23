using SmartBusinessManager.Core.Models;
using SmartBusinessManager.Core.Services;
using SmartBusinessManager.Features.Clients.Models;
using SmartBusinessManager.Core.Helpers;

namespace SmartBusinessManager.Features.Clients.ViewModels;

public class AddClientViewModel : BaseViewModel
{
    private readonly ISupabaseService _supabaseService;
    private readonly SessionManager _sessionManager;

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    private string _phone = string.Empty;
    public string Phone
    {
        get => _phone;
        set { _phone = value; OnPropertyChanged(); }
    }

    private string _company = string.Empty;
    public string Company
    {
        get => _company;
        set { _company = value; OnPropertyChanged(); }
    }

    private string _status = "active";
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    private string _notes = string.Empty;
    public string Notes
    {
        get => _notes;
        set { _notes = value; OnPropertyChanged(); }
    }

    private string _validationError;
    public string ValidationError
    {
        get => _validationError;
        set { _validationError = value; OnPropertyChanged(); }
    }

    public List<string> Statuses { get; } = new() { "active", "lead", "inactive", "archived" };

    public Command SaveCommand { get; }
    public Command CancelCommand { get; }

    public AddClientViewModel(ISupabaseService supabaseService, SessionManager sessionManager)
    {
        _supabaseService = supabaseService;
        _sessionManager = sessionManager;
        Title = "Add Client";

        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await CancelAsync());
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ValidationError = "Name is required.";
            return;
        }

        if (IsBusy) return;
        IsBusy = true;
        ValidationError = null;
        ErrorMessage = null;

        try
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                OwnerId = await _sessionManager.GetCurrentUserIdAsync(),
                Name = Name.Trim(),
                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                Company = string.IsNullOrWhiteSpace(Company) ? null : Company.Trim(),
                Status = Status.ToLowerInvariant(),
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _supabaseService.AddClientAsync(client);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to add client. Please check your data and try again.";
            MainThread.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert("Insert Error", ex.Message, "OK"));
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
