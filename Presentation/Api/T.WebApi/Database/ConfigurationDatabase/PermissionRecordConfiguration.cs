using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;
using T.Library.Model.Security;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public class PermissionRecordConfiguration : IEntityTypeConfiguration<PermissionRecord>
    {
        public void Configure(EntityTypeBuilder<PermissionRecord> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.PermissionRecordUserRoleMappings).WithOne(x => x.PermissionRecord).HasForeignKey(x => x.PermissionRecordId);
        }

    }
}
