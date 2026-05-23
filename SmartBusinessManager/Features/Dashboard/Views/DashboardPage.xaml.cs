using SmartBusinessManager.Features.Dashboard.ViewModels;

namespace SmartBusinessManager.Features.Dashboard.Views;

public partial class DashboardPage : ContentPage
{    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DashboardViewModel vm)
        {
            await vm.RefreshDashboardAsync();
        }
    }
}
