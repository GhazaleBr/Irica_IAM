using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SSO_Irica.Infrastructure.Persistence.Configurations;

public sealed class GenderConfiguration : IEntityTypeConfiguration<GenderRecord>
{
    public void Configure(EntityTypeBuilder<GenderRecord> b)
    {
        b.ToTable("Tbl_Gender"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Fld_Id"); b.Property(x => x.Code).HasColumnName("Fld_Code");
        b.Property(x => x.Title).HasColumnName("Fld_Title").HasMaxLength(200).IsRequired();
    }
}

public sealed class PositionConfiguration : IEntityTypeConfiguration<PositionRecord>
{
    public void Configure(EntityTypeBuilder<PositionRecord> b)
    {
        b.ToTable("Tbl_Positions"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Fld_Id"); b.Property(x => x.Code).HasColumnName("Fld_Code");
        b.Property(x => x.Title).HasColumnName("Fld_Title").HasMaxLength(200).IsRequired();
        b.Property(x => x.OrganizationUnitId).HasColumnName("Fld_Org/Unit_Id");
        b.Property(x => x.Description).HasColumnName("Fld_Description").HasMaxLength(500);
        b.Property(x => x.IsActive).HasColumnName("Fld_IsActive");
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class UserPositionConfiguration : IEntityTypeConfiguration<UserPositionRecord>
{
    public void Configure(EntityTypeBuilder<UserPositionRecord> b)
    {
        b.ToTable("Tbl_User_Positions"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Fld_Id"); b.Property(x => x.UserId).HasColumnName("Fld_User_Id");
        b.Property(x => x.PositionId).HasColumnName("Fld_Position_Id"); b.Property(x => x.IsPrimary).HasColumnName("Fld_IsPrimary");
        b.Property(x => x.StartDateTime).HasColumnName("Fld_StartDateTime"); b.Property(x => x.EndDateTime).HasColumnName("Fld_EndDateTime");
        b.Property(x => x.IsActive).HasColumnName("Fld_IsActive");
    }
}

public sealed class OrganizationUnitConfiguration : IEntityTypeConfiguration<OrganizationUnitRecord>
{
    public void Configure(EntityTypeBuilder<OrganizationUnitRecord> b)
    {
        b.ToTable("Tbl_Organization_Unit"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Fld_Id"); b.Property(x => x.ParentId).HasColumnName("Fld_Parent_Id");
        b.Property(x => x.Code).HasColumnName("Fld_Code"); b.Property(x => x.Title).HasColumnName("Fld_Title").HasMaxLength(200).IsRequired();
        b.Property(x => x.TypeId).HasColumnName("Fld_Type_Id"); b.Property(x => x.IsActive).HasColumnName("Fld_IsActive");
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class OrganizationTypeConfiguration : IEntityTypeConfiguration<OrganizationTypeRecord>
{
    public void Configure(EntityTypeBuilder<OrganizationTypeRecord> b)
    {
        b.ToTable("Tbl_Organization_Type"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Fld_Id"); b.Property(x => x.Code).HasColumnName("Fld_Code");
        b.Property(x => x.Title).HasColumnName("Fld_Title").HasMaxLength(50).IsRequired(); b.Property(x => x.IsActive).HasColumnName("Fld_IsActive");
        b.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class ActionLogConfiguration : IEntityTypeConfiguration<ActionLogRecord>
{
    public void Configure(EntityTypeBuilder<ActionLogRecord> b)
    {
        b.ToTable("Tbl_Actions_Log"); b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("Fld_Id"); b.Property(x => x.EntityId).HasColumnName("Fld_Entity_Id");
        b.Property(x => x.ModulesId).HasColumnName("Fld_Modules_Id");
    }
}
