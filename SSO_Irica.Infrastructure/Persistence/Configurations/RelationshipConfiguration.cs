using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSO_Irica.Domain.Identity;

namespace SSO_Irica.Infrastructure.Persistence.Configurations;

public sealed class RelationshipConfiguration :
    IEntityTypeConfiguration<UserRoleRecord>,
    IEntityTypeConfiguration<RolePermissionRecord>,
    IEntityTypeConfiguration<UserPositionRecord>,
    IEntityTypeConfiguration<PositionRecord>,
    IEntityTypeConfiguration<OrganizationUnitRecord>,
    IEntityTypeConfiguration<ActionLogRecord>,
    IEntityTypeConfiguration<ModuleRecord>
{
    public void Configure(EntityTypeBuilder<ModuleRecord> b) =>
        b.HasOne<ApplicationRecord>().WithMany().HasForeignKey(x => x.ApplicationId).OnDelete(DeleteBehavior.Restrict);

    public void Configure(EntityTypeBuilder<UserRoleRecord> b)
    {
        b.HasOne<SsoUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<RoleRecord>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<RolePermissionRecord> b)
    {
        b.HasOne<RoleRecord>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<ModuleRecord>().WithMany().HasForeignKey(x => x.ModuleId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PermissionRecord>().WithMany().HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<UserPositionRecord> b)
    {
        b.HasOne<SsoUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PositionRecord>().WithMany().HasForeignKey(x => x.PositionId).OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<PositionRecord> b) =>
        b.HasOne<OrganizationUnitRecord>().WithMany().HasForeignKey(x => x.OrganizationUnitId).OnDelete(DeleteBehavior.Restrict);

    public void Configure(EntityTypeBuilder<OrganizationUnitRecord> b)
    {
        b.HasOne<OrganizationTypeRecord>()
            .WithMany()
            .HasForeignKey(x => x.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<OrganizationUnitRecord>()
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<ActionLogRecord> b) =>
        b.HasOne<ModuleRecord>().WithMany().HasForeignKey(x => x.ModulesId).OnDelete(DeleteBehavior.Restrict);
}
