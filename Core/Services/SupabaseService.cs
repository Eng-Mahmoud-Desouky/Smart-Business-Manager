using SmartBusinessManager.Features.Clients.Models;
using SmartBusinessManager.Features.Finance.Models;
using SmartBusinessManager.Features.AI.Models;
using SmartBusinessManager.Core.Helpers;

namespace SmartBusinessManager.Core.Services;

public class SupabaseService : ISupabaseService
{
    private readonly SessionManager _sessionManager;

    public SupabaseService(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public Task<List<Client>> GetClientsAsync() => throw new NotImplementedException();
    public Task<Client> GetClientByIdAsync(string clientId) => throw new NotImplementedException();
    public Task AddClientAsync(Client client) => throw new NotImplementedException();
    public Task UpdateClientAsync(Client client) => throw new NotImplementedException();
    public Task DeleteClientAsync(string clientId) => throw new NotImplementedException();

    public Task<List<Payment>> GetPaymentsAsync() => throw new NotImplementedException();
    public Task<List<Payment>> GetPaymentsByClientAsync(string clientId) => throw new NotImplementedException();
    public Task AddPaymentAsync(Payment payment) => throw new NotImplementedException();
    public Task UpdatePaymentAsync(Payment payment) => throw new NotImplementedException();

    public Task<List<AiInsight>> GetInsightsAsync() => throw new NotImplementedException();
    public Task MarkInsightAsReadAsync(string insightId) => throw new NotImplementedException();
}
