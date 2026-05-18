namespace SmartBusinessManager.Core.Helpers;

public static class Constants
{
    // ── Supabase ──────────────────────────────────────────
    public const string SupabaseUrl     = "https://YOUR_PROJECT_ID.supabase.co";
    public const string SupabaseAnonKey = "YOUR_ANON_KEY_HERE";

    // ── Edge Functions ────────────────────────────────────
    public const string EdgeFunctionGenerateInsights =
        SupabaseUrl + "/functions/v1/generate-insights";

    // ── Supabase REST endpoints ───────────────────────────
    public const string RestBase      = SupabaseUrl + "/rest/v1";
    public const string AuthBase      = SupabaseUrl + "/auth/v1";

    // ── SecureStorage keys ────────────────────────────────
    public const string AccessTokenKey  = "sb_access_token";
    public const string RefreshTokenKey = "sb_refresh_token";
    public const string UserIdKey       = "sb_user_id";
}
