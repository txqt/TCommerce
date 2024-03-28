using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;
using T.Library.Model.Banners;
using T.Library.Model.Users;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class UserAddressMappingConfiguration : IEntityTypeConfiguration<UserAddressMapping>
    {
        public void Configure(EntityTypeBuilder<UserAddressMapping> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
