using SmartBusinessManager.Features.Clients.ViewModels;

namespace SmartBusinessManager.Features.Clients.Views;

public partial class ClientListPage : ContentPage
{    public ClientListPage(ClientListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
