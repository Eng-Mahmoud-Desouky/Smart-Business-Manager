using SmartBusinessManager.Core.Models;
using SmartBusinessManager.Core.Services;

namespace SmartBusinessManager.Features.Authentication.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged();
                RegisterCommand.ChangeCanExecute();
            }
        }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged();
                RegisterCommand.ChangeCanExecute();
            }
        }
    }

    public bool IsNotBusy => !IsBusy;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public Command RegisterCommand { get; }
    public Command NavigateToLoginCommand { get; }

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Create Account";

        RegisterCommand = new Command(
            async () => await RegisterAsync(),
            () => !IsBusy && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password)
        );

        NavigateToLoginCommand = new Command(
            async () => await NavigateToLoginAsync(),
            () => !IsBusy
        );
    }

    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(HasError));
        IsBusy = true;
        OnPropertyChanged(nameof(IsNotBusy));

        RegisterCommand.ChangeCanExecute();
        NavigateToLoginCommand.ChangeCanExecute();

        try
        {
            // Extract the prefix of the email to serve as the required 'full_name' for the profiles table
            string fullName = Email.Split('@')[0];

            var response = await _authService.SignUpAsync(Email, Password, fullName);
            if (response.IsSuccess)
            {
                await Shell.Current.GoToAsync("//dashboard");
            }
            else
            {
                ErrorMessage = response.ErrorMessage ?? "Registration failed. Please check your credentials.";
                OnPropertyChanged(nameof(HasError));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            OnPropertyChanged(nameof(HasError));
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(IsNotBusy));
            RegisterCommand.ChangeCanExecute();
            NavigateToLoginCommand.ChangeCanExecute();
        }
    }

    private async Task NavigateToLoginAsync()
    {
        await Shell.Current.GoToAsync("//login");
    }
}
