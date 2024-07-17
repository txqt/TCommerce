using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Catalogs;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class RelatedProductConfiguration : IEntityTypeConfiguration<RelatedProduct>
    {
        public void Configure(EntityTypeBuilder<RelatedProduct> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
