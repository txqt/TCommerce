using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TCommerce.Core.Models.Catalogs;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product review helpfulness entity builder
    /// </summary>
    public partial class ProductReviewHelpfulnessConfiguration : IEntityTypeConfiguration<ProductReviewHelpfulness>
    {
        public void Configure(EntityTypeBuilder<ProductReviewHelpfulness> builder)
        {
            builder.HasKey(x=>x.Id);
            builder.HasOne(x=>x.ProductReview).WithMany(x=>x.HelpfulHelpfulness).HasForeignKey(x=>x.ProductReviewId);
        }
    }
}