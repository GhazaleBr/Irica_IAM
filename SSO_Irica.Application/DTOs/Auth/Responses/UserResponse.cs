namespace SSO_Irica.Application.DTOs.Auth.Responses;

public sealed record UserResponse(Guid Id, string NationalCode, string FullName, string Mobile, string Role);
