using SmartBusinessManager.Features.Clients.Views;
using SmartBusinessManager.Features.Finance.Views;

namespace SmartBusinessManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ClientDetailPage), typeof(ClientDetailPage));
        Routing.RegisterRoute(nameof(AddClientPage),    typeof(AddClientPage));
        Routing.RegisterRoute(nameof(AddPaymentPage),   typeof(AddPaymentPage));
    }
}
