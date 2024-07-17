using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Users;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class UserAddressMappingConfiguration : IEntityTypeConfiguration<UserAddressMapping>
    {
        public void Configure(EntityTypeBuilder<UserAddressMapping> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
