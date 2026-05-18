using SmartBusinessManager.Features.Finance.ViewModels;

namespace SmartBusinessManager.Features.Finance.Views;

public partial class AddPaymentPage : ContentPage
{    public AddPaymentPage(AddPaymentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
