namespace SSO_Irica.Application.DTOs.Auth.Requests;

public sealed record VerifyTwoFactorRequest(string NationalCode, string Code);
