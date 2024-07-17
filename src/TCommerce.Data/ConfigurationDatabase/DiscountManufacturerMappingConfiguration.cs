using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Discounts;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class DiscountMappingConfiguration : IEntityTypeConfiguration<DiscountManufacturerMapping>
    {
        public void Configure(EntityTypeBuilder<DiscountManufacturerMapping> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
