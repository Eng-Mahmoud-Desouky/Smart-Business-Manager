using SmartBusinessManager.Features.Authentication.ViewModels;

namespace SmartBusinessManager.Features.Authentication.Views;

public partial class LoginPage : ContentPage
{    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
