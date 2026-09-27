using System.Text.Json.Serialization;

namespace SSO_Irica.Application.DTOs.Auth.Responses;

public sealed record TwoFactorChallengeResponse(string Message, int ExpiresInSeconds, string? DevelopmentCode)
{
    [JsonIgnore]
    public Guid UserId { get; init; }
}
