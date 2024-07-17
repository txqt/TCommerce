using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TCommerce.Core.Models.Catalogs;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product review entity builder
    /// </summary>
    public partial class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Product).WithMany(x => x.ProductReviews).HasForeignKey(x => x.ProductId);
        }
    }
}