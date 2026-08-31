using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SSO_Irica.Infrastructure.Persistence.Configurations;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRoleRecord>
{
    public void Configure(EntityTypeBuilder<UserRoleRecord> builder)
    {
        builder.ToTable("Tbl_User_Roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("Fld_Id");
        builder.Property(x => x.UserId).HasColumnName("Fld_User_Id");
        builder.Property(x => x.RoleId).HasColumnName("Fld_Role_Id");
    }
}
