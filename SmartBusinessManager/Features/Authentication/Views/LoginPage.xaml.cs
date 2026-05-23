using SmartBusinessManager.Features.Authentication.ViewModels;

namespace SmartBusinessManager.Features.Authentication.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LoginViewModel viewModel)
        {
            await viewModel.CheckSessionAndRedirectAsync();
        }
    }

    private void TogglePassword_Clicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        TogglePasswordBtn.Text = PasswordEntry.IsPassword ? "Show" : "Hide";
    }
}
