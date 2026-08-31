namespace SSO_Irica.Application.DTOs.Auth.Responses;

public sealed record AuthSession(
    string AccessToken,
    string RefreshToken,
    UserResponse User);
