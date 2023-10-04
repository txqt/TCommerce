using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T.Library.Model;

namespace T.WebApi.Database.ConfigurationDatabase
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
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        //public override void MapEntity(CreateTableExpressionBuilder table)
        //{
        //    table
        //        .WithColumn(nameof(ProductReview.CustomerId)).AsInt32().ForeignKey<Customer>()
        //        .WithColumn(nameof(ProductReview.ProductId)).AsInt32().ForeignKey<Product>()
        //        .WithColumn(nameof(ProductReview.StoreId)).AsInt32().ForeignKey<Store>();
        //}

        #endregion
    }
}