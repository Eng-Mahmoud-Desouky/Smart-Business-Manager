using SmartBusinessManager.Core.Helpers;

namespace SmartBusinessManager.Core.Services;

public class AiService : IAiService
{
    private readonly SessionManager _sessionManager;

    public AiService(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public Task<bool> GenerateInsightsAsync(string ownerId) => throw new NotImplementedException();
}
