using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class VietNamDistrictConfiguration : IEntityTypeConfiguration<VietNamDistrict>
    {
        public void Configure(EntityTypeBuilder<VietNamDistrict> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
