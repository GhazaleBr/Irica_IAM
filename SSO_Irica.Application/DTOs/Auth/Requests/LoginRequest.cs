namespace SSO_Irica.Application.DTOs.Auth.Requests;

public sealed record LoginRequest(string NationalCode, string Password);
