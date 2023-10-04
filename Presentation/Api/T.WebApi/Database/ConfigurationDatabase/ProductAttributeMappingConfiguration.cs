using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T.Library.Model;

namespace T.WebApi.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product attribute mapping entity builder
    /// </summary>
    public partial class ProductAttributeMappingConfiguration : IEntityTypeConfiguration<ProductAttributeMapping>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeMapping> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.ProductAttributeValue).WithOne(x => x.ProductAttributeMappings).HasForeignKey(x => x.ProductAttributeMappingId);
            builder.HasOne(x => x.Product).WithMany(x => x.AttributeMappings).HasForeignKey(x => x.ProductId);
        }
    }
}