using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class VietNamCommuneConfiguration : IEntityTypeConfiguration<VietNamCommune>
    {
        public void Configure(EntityTypeBuilder<VietNamCommune> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
