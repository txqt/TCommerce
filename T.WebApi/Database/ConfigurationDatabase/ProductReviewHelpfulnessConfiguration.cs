using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T.Library.Model;

namespace T.WebApi.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product review helpfulness entity builder
    /// </summary>
    public partial class ProductReviewHelpfulnessConfiguration : IEntityTypeConfiguration<ProductReviewHelpfulness>
    {
        public void Configure(EntityTypeBuilder<ProductReviewHelpfulness> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.ProductReview).WithMany(x=>x.HelpfulHelpfulness).HasForeignKey(x=>x.ProductReviewId);
        }
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        //public override void MapEntity(CreateTableExpressionBuilder table)
        //{
        //    table
        //        .WithColumn(nameof(ProductReviewHelpfulness.ProductReviewId)).AsInt32().ForeignKey<ProductReview>();
        //}

        #endregion
    }
}