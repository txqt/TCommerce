using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T.Library.Model.Seo;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public class UrlRecordConfiguration : IEntityTypeConfiguration<UrlRecord>
    {
        public void Configure(EntityTypeBuilder<UrlRecord> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
