
using SmartBusinessManager.Features.AI.ViewModels;

namespace SmartBusinessManager.Features.AI.Views;

public partial class InsightsPage : ContentPage
{
    private readonly InsightsViewModel _viewModel;

    public InsightsPage(InsightsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // تحميل التحليلات تلقائياً فور ظهور الشاشة أمام المستخدم
        if (_viewModel != null && _viewModel.LoadInsightsCommand.CanExecute(null))
        {
            _viewModel.LoadInsightsCommand.Execute(null);
        }
    }
}
