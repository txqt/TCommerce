using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Catalogs;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class RelatedProductConfiguration : IEntityTypeConfiguration<RelatedProduct>
    {
        public void Configure(EntityTypeBuilder<RelatedProduct> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
