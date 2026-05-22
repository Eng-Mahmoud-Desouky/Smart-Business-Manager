using SmartBusinessManager.Features.Clients.ViewModels;

namespace SmartBusinessManager.Features.Clients.Views;

public partial class ClientListPage : ContentPage
{    public ClientListPage(ClientListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ClientListViewModel vm)
        {
            await vm.LoadClientsAsync();
        }
    }
}
