using SmartBusinessManager.Features.AI.ViewModels;

namespace SmartBusinessManager.Features.AI.Views;

public partial class InsightsPage : ContentPage
{    public InsightsPage(InsightsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is InsightsViewModel vm)
        {
            await vm.LoadInsightsAsync();
        }
    }
}
