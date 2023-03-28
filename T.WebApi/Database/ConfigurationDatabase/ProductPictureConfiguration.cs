using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T.Library.Model;

namespace T.WebApi.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product picture entity builder
    /// </summary>
    public partial class ProductPictureConfiguration : IEntityTypeConfiguration<ProductPicture>
    {
        public void Configure(EntityTypeBuilder<ProductPicture> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Picture).WithMany(x => x.ProductPictures).HasForeignKey(x=>x.PictureId);
            builder.HasOne(x => x.Product).WithMany(x => x.ProductPictures).HasForeignKey(x=>x.ProductId);
        }
    }
}