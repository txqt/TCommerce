using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T.Library.Model.Catalogs;

namespace T.WebApi.Database.ConfigurationDatabase
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
