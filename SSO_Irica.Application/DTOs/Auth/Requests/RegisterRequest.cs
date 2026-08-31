namespace SSO_Irica.Application.DTOs.Auth.Requests;

public sealed record RegisterRequest(string FullName, string NationalCode, string Mobile, string Password);
