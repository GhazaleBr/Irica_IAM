using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SSO_Irica.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<RoleRecord>
{
    public void Configure(EntityTypeBuilder<RoleRecord> builder)
    {
        builder.ToTable("Tbl_Roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("Fld_Id");
        builder.Property(x => x.Code).HasColumnName("Fld_Code");
        builder.Property(x => x.Title).HasColumnName("Fld_Title").HasMaxLength(200);
        builder.Property(x => x.Description).HasColumnName("Fld_Description").HasMaxLength(500);
        builder.Property(x => x.IsActive).HasColumnName("Fld_IsActive");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
