using SmartBusinessManager.Features.Clients.Views;
using SmartBusinessManager.Features.Finance.Views;

namespace SmartBusinessManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("clients/detail", typeof(ClientDetailPage));
        Routing.RegisterRoute("clients/add",    typeof(AddClientPage));
        Routing.RegisterRoute("payments/add",   typeof(AddPaymentPage));
    }
}
