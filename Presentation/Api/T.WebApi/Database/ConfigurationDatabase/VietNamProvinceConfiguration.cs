using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class VietNamProvinceConfiguration : IEntityTypeConfiguration<VietNamProvince>
    {
        public void Configure(EntityTypeBuilder<VietNamProvince> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
