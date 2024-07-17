using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TCommerce.Core.Models.Seo;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public class UrlRecordConfiguration : IEntityTypeConfiguration<UrlRecord>
    {
        public void Configure(EntityTypeBuilder<UrlRecord> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
