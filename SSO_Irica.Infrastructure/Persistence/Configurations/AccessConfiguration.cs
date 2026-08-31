using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SSO_Irica.Infrastructure.Persistence.Configurations;

public sealed class ApplicationConfiguration : IEntityTypeConfiguration<ApplicationRecord>
{
    public void Configure(EntityTypeBuilder<ApplicationRecord> b)
    {
        b.ToTable("Tbl_Application"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Fld_Id");
        b.Property(x => x.Code).HasColumnName("Fld_Code").HasMaxLength(100).IsRequired();
        b.Property(x => x.Title).HasColumnName("Fld_Title").HasMaxLength(100).IsRequired();
        b.Property(x => x.IsActive).HasColumnName("Fld_IsActive").IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class ModuleConfiguration : IEntityTypeConfiguration<ModuleRecord>
{
    public void Configure(EntityTypeBuilder<ModuleRecord> b)
    {
        b.ToTable("Tbl_Modules"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Fld_Id");
        b.Property(x => x.Code).HasColumnName("Fld_Code").HasMaxLength(100).IsRequired();
        b.Property(x => x.Title).HasColumnName("Fld_Title").HasMaxLength(100).IsRequired();
        b.Property(x => x.ApplicationId).HasColumnName("Fld_Application_Id").IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class PermissionConfiguration : IEntityTypeConfiguration<PermissionRecord>
{
    public void Configure(EntityTypeBuilder<PermissionRecord> b)
    {
        b.ToTable("Tbl_Permissions"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Fld_Id");
        b.Property(x => x.Code).HasColumnName("Fld_Code").HasMaxLength(100).IsRequired();
        b.Property(x => x.Title).HasColumnName("Fld_Title").HasMaxLength(200).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermissionRecord>
{
    public void Configure(EntityTypeBuilder<RolePermissionRecord> b)
    {
        b.ToTable("Tbl_Role_Permissions"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Fld_Id");
        b.Property(x => x.RoleId).HasColumnName("Fld_Role_Id");
        b.Property(x => x.ModuleId).HasColumnName("Fld_Module_Id");
        b.Property(x => x.PermissionId).HasColumnName("Fld_Permision_Id");
        b.Property(x => x.IsActive).HasColumnName("Fld_IsActive");
        b.HasIndex(x => new { x.RoleId, x.ModuleId, x.PermissionId }).IsUnique();
    }
}
