namespace SSO_Irica.Application.DTOs.Access.Requests;

public sealed record CreateApplicationRequest(string Code, string Title);
public sealed record UpdateApplicationRequest(string Code, string Title);
public sealed record CreateModuleRequest(string Code, string Title, int ApplicationId);
public sealed record UpdateModuleRequest(string Code, string Title, int ApplicationId);
