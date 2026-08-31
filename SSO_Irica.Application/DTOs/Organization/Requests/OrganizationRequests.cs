namespace SSO_Irica.Application.DTOs.Organization.Requests;

public sealed record CreateOrganizationTypeRequest(int Code, string Title);
public sealed record UpdateOrganizationTypeRequest(int Code, string Title);
public sealed record CreateOrganizationUnitRequest(int Code, string Title, int TypeId, int? ParentId = null);
public sealed record UpdateOrganizationUnitRequest(int Code, string Title, int TypeId, int? ParentId = null);
public sealed record CreatePositionRequest(int Code, string Title, int OrganizationUnitId, string? Description = null);
public sealed record UpdatePositionRequest(int Code, string Title, int OrganizationUnitId, string? Description = null);
public sealed record CreateGenderRequest(int Code, string Title);
public sealed record UpdateGenderRequest(int Code, string Title);
public sealed record CreateUserPositionRequest(Guid UserId, int PositionId, bool IsPrimary, DateTime StartDateTime, DateTime? EndDateTime = null);
public sealed record UpdateUserPositionRequest(int PositionId, bool IsPrimary, DateTime StartDateTime, DateTime? EndDateTime = null);
