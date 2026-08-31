namespace SSO_Irica.Application.DTOs.Access.Responses;

public sealed record ApplicationResponse(int Id, string Code, string Title, bool IsActive);
public sealed record ModuleResponse(int Id, string Code, string Title, int ApplicationId);
