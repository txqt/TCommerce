using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TCommerce.Core.Models.Catalogs;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product attribute entity builder
    /// </summary>
    public partial class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(int.MaxValue);
            builder.HasMany(x=>x.ProductAttributeMappings).WithOne(x=>x.ProductAttribute).HasForeignKey(x=>x.ProductAttributeId);
        }
    }
}