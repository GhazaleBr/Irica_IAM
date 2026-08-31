using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSO_Irica.Domain.Identity;

namespace SSO_Irica.Infrastructure.Persistence.Configurations;

public sealed class SsoUserConfiguration : IEntityTypeConfiguration<SsoUser>
{
    public void Configure(EntityTypeBuilder<SsoUser> builder)
    {
        builder.ToTable("Tbl_User");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("Fld_Id");
        builder.Property(x => x.NationalCodeValue).HasColumnName("Fld_UserName").HasMaxLength(100).IsRequired();
        builder.Property(x => x.PasswordHash).HasColumnName("Fld_Password").HasMaxLength(512).IsRequired();
        builder.Property(x => x.FirstName).HasColumnName("Fld_FirstName").HasMaxLength(200).IsRequired();
        builder.Property(x => x.LastName).HasColumnName("Fld_LastName").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Mobile).HasColumnName("Fld_Mobile").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Email).HasColumnName("Fld_Email").HasMaxLength(256).IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("Fld_IsActive").IsRequired();
        builder.Property(x => x.GenderId).HasColumnName("Fld_Gender");
        builder.Ignore(x => x.FullName);
        builder.Ignore(x => x.Role);
        builder.HasIndex(x => x.NationalCodeValue).IsUnique();
    }
}
