namespace SSO_Irica.Infrastructure.ExternalServices;

public sealed class ExternalApiOptions
{
    public const string SectionName = "ExternalApis";
    public string IdentityVerificationBaseUrl { get; init; } = string.Empty;
    public string IdentityVerificationPath { get; init; } = "/verify";
    public string SmsBaseUrl { get; init; } = string.Empty;
    public string SmsPath { get; init; } = "/send";
    public string ApiKey { get; init; } = string.Empty;
}
