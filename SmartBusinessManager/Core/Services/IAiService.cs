namespace SmartBusinessManager.Core.Services;

public interface IAiService
{
    Task<bool> GenerateInsightsAsync(string ownerId);
}
