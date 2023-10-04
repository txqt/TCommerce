using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T.Library.Model;

namespace T.WebApi.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product entity builder
    /// </summary>
    public partial class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(400);
            //builder.Property(x => x.MetaKeywords).IsRequired().HasMaxLength(400);
            //builder.Property(x => x.MetaTitle).IsRequired().HasMaxLength(400);
            //builder.Property(x => x.Sku).IsRequired().HasMaxLength(400);
            //builder.Property(x => x.ManufacturerPartNumber).IsRequired().HasMaxLength(400);
        }
    }
}