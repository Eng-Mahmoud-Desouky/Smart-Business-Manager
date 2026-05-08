namespace SmartBusinessManager.Core.Helpers;

public class SessionManager
{
    public string AccessToken  => SecureStorage.GetAsync(Constants.AccessTokenKey).Result;
    public string CurrentUserId => SecureStorage.GetAsync(Constants.UserIdKey).Result;

    public async Task SaveSessionAsync(string accessToken, string refreshToken, string userId)
    {
        await SecureStorage.SetAsync(Constants.AccessTokenKey,  accessToken);
        await SecureStorage.SetAsync(Constants.RefreshTokenKey, refreshToken);
        await SecureStorage.SetAsync(Constants.UserIdKey,       userId);
    }

    public void ClearSession()
    {
        SecureStorage.Remove(Constants.AccessTokenKey);
        SecureStorage.Remove(Constants.RefreshTokenKey);
        SecureStorage.Remove(Constants.UserIdKey);
    }

    public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken);
}
