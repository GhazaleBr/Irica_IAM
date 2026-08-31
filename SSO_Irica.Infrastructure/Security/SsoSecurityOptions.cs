namespace SSO_Irica.Infrastructure.Security;

public sealed class SsoSecurityOptions
{
    public const string SectionName = "SsoSecurity";
    public string RefreshTokenCookieName { get; set; } = "__Host-irika_refresh";
    public int RefreshTokenDays { get; set; } = 7;
    public bool SecureCookie { get; set; } = true;
}
