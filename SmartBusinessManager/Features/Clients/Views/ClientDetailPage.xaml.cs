using SmartBusinessManager.Features.Clients.ViewModels;

namespace SmartBusinessManager.Features.Clients.Views;

public partial class ClientDetailPage : ContentPage
{    public ClientDetailPage(ClientDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ClientDetailViewModel vm)
        {
            await vm.LoadDetailsAsync();
        }
    }
}
