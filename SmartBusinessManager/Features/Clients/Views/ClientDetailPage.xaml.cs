using SmartBusinessManager.Features.Clients.ViewModels;

namespace SmartBusinessManager.Features.Clients.Views;

public partial class ClientDetailPage : ContentPage
{    public ClientDetailPage(ClientDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
