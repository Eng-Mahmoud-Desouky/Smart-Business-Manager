using SmartBusinessManager.Features.Dashboard.ViewModels;

namespace SmartBusinessManager.Features.Dashboard.Views;

public partial class DashboardPage : ContentPage
{    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
