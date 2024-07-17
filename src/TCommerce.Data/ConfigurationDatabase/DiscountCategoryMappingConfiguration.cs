using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Discounts;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class DiscountCategoryMappingConfiguration : IEntityTypeConfiguration<DiscountCategoryMapping>
    {
        public void Configure(EntityTypeBuilder<DiscountCategoryMapping> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
