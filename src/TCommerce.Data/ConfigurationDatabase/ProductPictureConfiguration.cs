using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product picture entity builder
    /// </summary>
    public partial class ProductPictureConfiguration : IEntityTypeConfiguration<ProductPicture>
    {
        public void Configure(EntityTypeBuilder<ProductPicture> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<Picture>().WithMany().HasForeignKey(x=>x.PictureId);
            builder.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId);
        }
    }
}