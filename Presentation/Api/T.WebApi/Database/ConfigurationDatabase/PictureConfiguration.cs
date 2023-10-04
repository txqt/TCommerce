using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T.Library.Model.Common;

namespace T.WebApi.Database.ConfigurationDatabase
{
    /// <summary>
    /// Represents a product attribute entity builder
    /// </summary>
    public partial class PictureConfigurationConfiguration : IEntityTypeConfiguration<Picture>
    {
        public void Configure(EntityTypeBuilder<Picture> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}