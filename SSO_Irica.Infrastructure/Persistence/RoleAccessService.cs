using Microsoft.EntityFrameworkCore;
using SSO_Irica.Application.Abstractions;
using SSO_Irica.Application.DTOs.Access;

namespace SSO_Irica.Infrastructure.Persistence;

public sealed class RoleAccessService(SsoDbContext db) : IRoleAccessService
{
    public async Task<int> CreateRoleAsync(int code, string title, string? description, CancellationToken cancellationToken = default)
    {
        var role = new RoleRecord { Code = code, Title = title.Trim(), Description = description?.Trim(), IsActive = true };
        db.Roles.Add(role);
        await db.SaveChangesAsync(cancellationToken);
        return role.Id;
    }

    public async Task AssignRoleAsync(int roleId, Guid userId, CancellationToken cancellationToken = default)
    {
        var exists = await db.UserRoles.AnyAsync(x => x.RoleId == roleId && x.UserId == userId, cancellationToken);
        if (!exists)
        {
            db.UserRoles.Add(new UserRoleRecord { RoleId = roleId, UserId = userId });
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task ReplaceAccessAsync(int roleId, IReadOnlyList<RoleAccessItem> items, CancellationToken cancellationToken = default)
    {
        var current = await db.RolePermissions.Where(x => x.RoleId == roleId).ToListAsync(cancellationToken);
        db.RolePermissions.RemoveRange(current);
        db.RolePermissions.AddRange(items.Select(x => new RolePermissionRecord
        {
            RoleId = roleId, ModuleId = x.ModuleId, PermissionId = x.PermissionId, IsActive = x.IsActive
        }));
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RoleAccessItem>> GetUserAccessAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await (
            from assignment in db.UserRoles.AsNoTracking()
            join access in db.RolePermissions.AsNoTracking()
                on assignment.RoleId equals access.RoleId
            where assignment.UserId == userId && access.IsActive
            select new RoleAccessItem(access.ModuleId, access.PermissionId, access.IsActive))
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AccessCatalogItem>> GetModulesAsync(CancellationToken cancellationToken = default) =>
        await db.Modules.AsNoTracking().OrderBy(x => x.Title)
            .Select(x => new AccessCatalogItem(x.Id, x.Code, x.Title)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AccessCatalogItem>> GetPermissionsAsync(CancellationToken cancellationToken = default) =>
        await db.Permissions.AsNoTracking().OrderBy(x => x.Title)
            .Select(x => new AccessCatalogItem(x.Id, x.Code, x.Title)).ToListAsync(cancellationToken);

    public async Task<int> CreateModuleAsync(string code, string title, CancellationToken cancellationToken = default)
    {
        var entity = new ModuleRecord { Code = code.Trim(), Title = title.Trim() };
        db.Modules.Add(entity); await db.SaveChangesAsync(cancellationToken); return entity.Id;
    }

    public async Task<int> CreatePermissionAsync(string code, string title, CancellationToken cancellationToken = default)
    {
        var entity = new PermissionRecord { Code = code.Trim(), Title = title.Trim() };
        db.Permissions.Add(entity); await db.SaveChangesAsync(cancellationToken); return entity.Id;
    }
}
