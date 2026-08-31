namespace SSO_Irica.Application.DTOs.Auth.Responses;

public sealed record TwoFactorChallengeResponse(string Message, int ExpiresInSeconds, string? DevelopmentCode);
