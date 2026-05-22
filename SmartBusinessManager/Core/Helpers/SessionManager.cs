namespace SmartBusinessManager.Core.Helpers;

public class SessionManager
{
    private string _accessToken;
    private string _currentUserId;
    private bool _isLoaded;

    public string AccessToken
    {
        get
        {
            if (!_isLoaded)
            {
                LoadSessionSync();
            }
            return _accessToken;
        }
    }

    public string CurrentUserId
    {
        get
        {
            if (!_isLoaded)
            {
                LoadSessionSync();
            }
            return _currentUserId;
        }
    }

    private void LoadSessionSync()
    {
        try
        {
            // First try SecureStorage (can deadlock if not careful, but we must run synchronously here)
            // Using a safe background thread waiter to avoid UI deadlocks
            var task = Task.Run(async () => 
            {
                try { return await SecureStorage.GetAsync(Constants.AccessTokenKey); }
                catch { return Preferences.Get(Constants.AccessTokenKey, null); }
            });
            _accessToken = task.ConfigureAwait(false).GetAwaiter().GetResult();

            var userTask = Task.Run(async () => 
            {
                try { return await SecureStorage.GetAsync(Constants.UserIdKey); }
                catch { return Preferences.Get(Constants.UserIdKey, null); }
            });
            _currentUserId = userTask.ConfigureAwait(false).GetAwaiter().GetResult();

            _isLoaded = true;
        }
        catch
        {
            // Ultimate fallback
            _accessToken = Preferences.Get(Constants.AccessTokenKey, null);
            _currentUserId = Preferences.Get(Constants.UserIdKey, null);
            _isLoaded = true;
        }
    }

    public async Task<bool> CheckSessionAsync()
    {
        try
        {
            try
            {
                _accessToken = await SecureStorage.GetAsync(Constants.AccessTokenKey);
                _currentUserId = await SecureStorage.GetAsync(Constants.UserIdKey);
            }
            catch
            {
                // Fallback to Preferences on Keystore failure
                _accessToken = Preferences.Get(Constants.AccessTokenKey, null);
                _currentUserId = Preferences.Get(Constants.UserIdKey, null);
            }
            
            _isLoaded = true;
            return !string.IsNullOrEmpty(_accessToken);
        }
        catch
        {
            return false;
        }
    }

    public async Task SaveSessionAsync(string accessToken, string refreshToken, string userId)
    {
        try
        {
            await SecureStorage.SetAsync(Constants.AccessTokenKey, accessToken);
            await SecureStorage.SetAsync(Constants.RefreshTokenKey, refreshToken);
            await SecureStorage.SetAsync(Constants.UserIdKey, userId);
        }
        catch
        {
            // Ignore SecureStorage errors and just rely on Preferences below
        }

        // Always save to Preferences as a safe fallback for Xiaomi/MIUI bugs
        Preferences.Set(Constants.AccessTokenKey, accessToken);
        Preferences.Set(Constants.RefreshTokenKey, refreshToken);
        Preferences.Set(Constants.UserIdKey, userId);
        
        _accessToken = accessToken;
        _currentUserId = userId;
        _isLoaded = true;
    }

    public void ClearSession()
    {
        SecureStorage.Remove(Constants.AccessTokenKey);
        SecureStorage.Remove(Constants.RefreshTokenKey);
        SecureStorage.Remove(Constants.UserIdKey);
        
        Preferences.Remove(Constants.AccessTokenKey);
        Preferences.Remove(Constants.RefreshTokenKey);
        Preferences.Remove(Constants.UserIdKey);

        _accessToken = null;
        _currentUserId = null;
        _isLoaded = true;
    }

    public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken);
}

