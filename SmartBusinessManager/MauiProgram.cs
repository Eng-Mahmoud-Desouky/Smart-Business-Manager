using CommunityToolkit.Maui;
using SmartBusinessManager.Core.Helpers;
using SmartBusinessManager.Core.Services;
using SmartBusinessManager.Features.Authentication.ViewModels;
using SmartBusinessManager.Features.Authentication.Views;
using SmartBusinessManager.Features.Dashboard.ViewModels;
using SmartBusinessManager.Features.Dashboard.Views;
using SmartBusinessManager.Features.Clients.ViewModels;
using SmartBusinessManager.Features.Clients.Views;
using SmartBusinessManager.Features.Finance.ViewModels;
using SmartBusinessManager.Features.Finance.Views;
using SmartBusinessManager.Features.AI.ViewModels;
using SmartBusinessManager.Features.AI.Views;
using SmartBusinessManager.Features.TempShowcase.Views;

namespace SmartBusinessManager;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            System.Diagnostics.Debug.WriteLine($"\n\n====================\nFATAL MAUI CRASH: {error.ExceptionObject}\n====================\n\n");
            Console.WriteLine($"\n\n====================\nFATAL MAUI CRASH: {error.ExceptionObject}\n====================\n\n");
        };

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── Core (Singleton) ──────────────────────────────
        builder.Services.AddSingleton<SessionManager>();
        builder.Services.AddSingleton<IAuthService,      AuthService>();
        builder.Services.AddSingleton<ISupabaseService,  SupabaseService>();
        builder.Services.AddSingleton<IAiService,        AiService>();

        // ── ViewModels (Transient) ────────────────────────
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<ClientListViewModel>();
        builder.Services.AddTransient<ClientDetailViewModel>();
        builder.Services.AddTransient<PaymentListViewModel>();
        builder.Services.AddTransient<AddPaymentViewModel>();
        builder.Services.AddTransient<InsightsViewModel>();

        // ── Pages (Transient) ─────────────────────────────
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ClientListPage>();
        builder.Services.AddTransient<ClientDetailPage>();
        builder.Services.AddTransient<AddClientPage>();
        builder.Services.AddTransient<PaymentListPage>();
        builder.Services.AddTransient<AddPaymentPage>();
        builder.Services.AddTransient<InsightsPage>();
        builder.Services.AddTransient<ComponentShowcasePage>();

        return builder.Build();
    }
}
