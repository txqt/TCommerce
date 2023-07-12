using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Security;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public class PermissionRecordUserRoleMappingConfiguration : IEntityTypeConfiguration<PermissionRecordUserRoleMapping>
    {
        public void Configure(EntityTypeBuilder<PermissionRecordUserRoleMapping> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Role).WithMany(x => x.PermissionRecordUserRoleMappings).HasForeignKey(x => x.UserRoleId);
            builder.HasOne(x => x.PermissionRecord).WithMany(x => x.PermissionRecordUserRoleMappings).HasForeignKey(x => x.PermissionRecordId);
        }

    }
}
