using SmartBusinessManager.Features.Clients.Models;
using SmartBusinessManager.Features.Finance.Models;
using SmartBusinessManager.Features.AI.Models;

namespace SmartBusinessManager.Core.Services;

public interface ISupabaseService
{
    // Clients
    Task<List<Client>>  GetClientsAsync();
    Task<Client>        GetClientByIdAsync(string clientId);
    Task                AddClientAsync(Client client);
    Task                UpdateClientAsync(Client client);
    Task                DeleteClientAsync(string clientId);

    // Payments
    Task<List<Payment>> GetPaymentsAsync();
    Task<List<Payment>> GetPaymentsByClientAsync(string clientId);
    Task                AddPaymentAsync(Payment payment);
    Task                UpdatePaymentAsync(Payment payment);

    // AI Insights
    Task<List<AiInsight>> GetInsightsAsync();
    Task                  MarkInsightAsReadAsync(string insightId);

    // Interactions
    Task<List<Interaction>> GetInteractionsByClientAsync(string clientId);
    Task                    AddInteractionAsync(Interaction interaction);
}
