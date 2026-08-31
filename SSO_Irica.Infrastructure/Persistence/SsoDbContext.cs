using Microsoft.EntityFrameworkCore;
using SSO_Irica.Domain.Identity;

namespace SSO_Irica.Infrastructure.Persistence;

public sealed class SsoDbContext(DbContextOptions<SsoDbContext> options) : DbContext(options)
{
    public DbSet<SsoUser> Users => Set<SsoUser>();
    public DbSet<RoleRecord> Roles => Set<RoleRecord>();
    public DbSet<UserRoleRecord> UserRoles => Set<UserRoleRecord>();
    public DbSet<ApplicationRecord> Applications => Set<ApplicationRecord>();
    public DbSet<ModuleRecord> Modules => Set<ModuleRecord>();
    public DbSet<PermissionRecord> Permissions => Set<PermissionRecord>();
    public DbSet<GenderRecord> Genders => Set<GenderRecord>();
    public DbSet<PositionRecord> Positions => Set<PositionRecord>();
    public DbSet<UserPositionRecord> UserPositions => Set<UserPositionRecord>();
    public DbSet<OrganizationUnitRecord> OrganizationUnits => Set<OrganizationUnitRecord>();
    public DbSet<OrganizationTypeRecord> OrganizationTypes => Set<OrganizationTypeRecord>();
    public DbSet<ActionLogRecord> ActionLogs => Set<ActionLogRecord>();
    public DbSet<RolePermissionRecord> RolePermissions => Set<RolePermissionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SsoDbContext).Assembly);
}
