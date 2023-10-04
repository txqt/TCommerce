using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class CategoryConfigurationConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
