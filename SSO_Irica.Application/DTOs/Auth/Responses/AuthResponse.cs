namespace SSO_Irica.Application.DTOs.Auth.Responses;

public sealed record AuthResponse(string AccessToken, UserResponse User);
