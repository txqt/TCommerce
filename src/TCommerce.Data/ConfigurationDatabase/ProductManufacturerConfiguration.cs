using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TCommerce.Core.Models.Catalogs;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class ProductManufacturerConfiguration : IEntityTypeConfiguration<ProductManufacturer>
    {
        public void Configure(EntityTypeBuilder<ProductManufacturer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Product).WithMany(x => x.ProductManufacturers).HasForeignKey(x=>x.ProductId);
            builder.HasOne(x => x.Manufacturer).WithMany(x => x.ProductManufacturers).HasForeignKey(x=>x.ManufacturerId);
        }
    }
}
