using SmartBusinessManager.Features.Clients.ViewModels;

namespace SmartBusinessManager.Features.Clients.Views;

public partial class AddClientPage : ContentPage
{
    public AddClientPage(AddClientViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
