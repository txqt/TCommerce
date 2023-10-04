using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T.Library.Model;

namespace T.WebApi.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product attribute value entity builder
    /// </summary>
    public partial class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(400).IsRequired();
            //builder.Property(x => x.ColorSquaresRgb).IsRequired().HasMaxLength(400);
            //builder.HasOne(x => x.ProductAttributeMappings).WithMany(x => x.ProductAttributeValue).HasForeignKey(x => x.ProductAttributeId);
        }
    }
}