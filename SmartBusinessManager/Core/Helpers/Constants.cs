namespace SmartBusinessManager.Core.Helpers;

public static class Constants
{
    // ── Supabase ──────────────────────────────────────────
    public const string SupabaseUrl     = "https://htiqetrzfbdseqborjxw.supabase.co";
    public const string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Imh0aXFldHJ6ZmJkc2VxYm9yanh3Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3Nzc3MDQ2NzUsImV4cCI6MjA5MzI4MDY3NX0.s0nfewie5VgH2MjoJA3ghCCOQeILQULu-XLNHvCVOCY";

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
