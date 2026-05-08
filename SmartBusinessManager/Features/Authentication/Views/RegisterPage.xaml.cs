using SmartBusinessManager.Features.Authentication.ViewModels;

namespace SmartBusinessManager.Features.Authentication.Views;

public partial class RegisterPage : ContentPage
{    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
