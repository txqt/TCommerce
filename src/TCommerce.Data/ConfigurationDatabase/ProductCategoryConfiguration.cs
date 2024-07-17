

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TCommerce.Core.Models.Catalogs;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product category entity builder
    /// </summary>
    public partial class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<Category>().WithMany().HasForeignKey(x => x.CategoryId);
            builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId);
        }

    }
}