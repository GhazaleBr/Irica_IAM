using SSO_Irica.Application.DTOs.Access;

namespace SSO_Irica.Application.Abstractions;

public interface IRoleAccessService
{
    Task<int> CreateRoleAsync(int code, string title, string? description, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(int roleId, Guid userId, CancellationToken cancellationToken = default);
    Task ReplaceAccessAsync(int roleId, IReadOnlyList<RoleAccessItem> items, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleAccessItem>> GetUserAccessAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccessCatalogItem>> GetModulesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccessCatalogItem>> GetPermissionsAsync(CancellationToken cancellationToken = default);
    Task<int> CreateModuleAsync(string code, string title, CancellationToken cancellationToken = default);
    Task<int> CreatePermissionAsync(string code, string title, CancellationToken cancellationToken = default);
}
