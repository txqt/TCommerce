using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Banners;
using T.Library.Model.Discounts;
using T.Library.Model.Users;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class DiscountMappingConfiguration : IEntityTypeConfiguration<DiscountMapping>
    {
        public void Configure(EntityTypeBuilder<DiscountMapping> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
