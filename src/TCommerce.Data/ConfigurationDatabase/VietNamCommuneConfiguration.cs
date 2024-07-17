using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class VietNamCommuneConfiguration : IEntityTypeConfiguration<VietNamCommune>
    {
        public void Configure(EntityTypeBuilder<VietNamCommune> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
