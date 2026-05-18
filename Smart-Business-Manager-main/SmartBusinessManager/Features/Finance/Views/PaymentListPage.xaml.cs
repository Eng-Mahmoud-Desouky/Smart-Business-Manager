using SmartBusinessManager.Features.Finance.ViewModels;

namespace SmartBusinessManager.Features.Finance.Views;

public partial class PaymentListPage : ContentPage
{    public PaymentListPage(PaymentListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
