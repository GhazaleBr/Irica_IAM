namespace SSO_Irica.Application.DTOs.Organization.Responses;

public sealed record OrganizationTypeResponse(int Id, int Code, string Title, bool IsActive);
public sealed record OrganizationUnitResponse(int Id, int Code, string Title, int TypeId, int? ParentId, bool IsActive);
public sealed record PositionResponse(int Id, int Code, string Title, int OrganizationUnitId, string? Description, bool IsActive);
public sealed record GenderResponse(int Id, int Code, string Title);
public sealed record UserPositionResponse(int Id, Guid UserId, int PositionId, bool IsPrimary, DateTime StartDateTime, DateTime? EndDateTime, bool IsActive);
