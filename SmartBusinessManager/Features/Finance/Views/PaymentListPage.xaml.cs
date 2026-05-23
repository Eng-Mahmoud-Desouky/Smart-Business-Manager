using SmartBusinessManager.Features.Finance.ViewModels;

namespace SmartBusinessManager.Features.Finance.Views;

public partial class PaymentListPage : ContentPage
{    public PaymentListPage(PaymentListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PaymentListViewModel vm)
        {
            await vm.LoadPaymentsAsync();
        }
    }
}
