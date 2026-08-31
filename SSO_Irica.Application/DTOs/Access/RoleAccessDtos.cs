namespace SSO_Irica.Application.DTOs.Access;

public sealed record RoleAccessItem(int ModuleId, int PermissionId, bool IsActive = true);
public sealed record CreateRoleRequest(int Code, string Title, string? Description = null);
public sealed record AssignRoleRequest(Guid UserId);
public sealed record ReplaceRoleAccessRequest(IReadOnlyList<RoleAccessItem> Items);
public sealed record RoleAccessResponse(int RoleId, string Title, IReadOnlyList<RoleAccessItem> Items);
public sealed record AccessCatalogItem(int Id, string Code, string Title);
public sealed record CreateAccessCatalogRequest(string Code, string Title);
