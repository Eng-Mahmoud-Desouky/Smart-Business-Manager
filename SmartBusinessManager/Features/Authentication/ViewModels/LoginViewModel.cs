using SmartBusinessManager.Core.Models;
using SmartBusinessManager.Core.Services;
using SmartBusinessManager.Core.Helpers;

namespace SmartBusinessManager.Features.Authentication.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly SessionManager _sessionManager;

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
                LoginCommand.ChangeCanExecute();
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
                LoginCommand.ChangeCanExecute();
            }
        }
    }

    public bool IsNotBusy => !IsBusy;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public Command LoginCommand { get; }
    public Command NavigateToRegisterCommand { get; }

    public LoginViewModel(IAuthService authService, SessionManager sessionManager)
    {
        _authService = authService;
        _sessionManager = sessionManager;
        Title = "Sign In";

        LoginCommand = new Command(
            async () => await LoginAsync(),
            () => !IsBusy && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password)
        );

        NavigateToRegisterCommand = new Command(
            async () => await NavigateToRegisterAsync(),
            () => !IsBusy
        );
    }

    public async Task CheckSessionAndRedirectAsync()
    {
        IsBusy = true;
        OnPropertyChanged(nameof(IsNotBusy));
        try
        {
            var hasSession = await _sessionManager.CheckSessionAsync();
            if (hasSession)
            {
                await Shell.Current.GoToAsync("//dashboard");
            }
        }
        catch
        {
            // Fail silently, let user log in manually
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }


    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(HasError));
        IsBusy = true;
        OnPropertyChanged(nameof(IsNotBusy));
        
        LoginCommand.ChangeCanExecute();
        NavigateToRegisterCommand.ChangeCanExecute();

        try
        {
            var response = await _authService.SignInAsync(Email, Password);
            if (response.IsSuccess)
            {
                await Shell.Current.GoToAsync("//dashboard");
            }
            else
            {
                ErrorMessage = response.ErrorMessage ?? "Invalid login credentials.";
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
            LoginCommand.ChangeCanExecute();
            NavigateToRegisterCommand.ChangeCanExecute();
        }
    }

    private async Task NavigateToRegisterAsync()
    {
        await Shell.Current.GoToAsync("//register");
    }
}
