using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Discounts;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class DiscountProductMappingConfiguration : IEntityTypeConfiguration<DiscountProductMapping>
    {
        public void Configure(EntityTypeBuilder<DiscountProductMapping> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
