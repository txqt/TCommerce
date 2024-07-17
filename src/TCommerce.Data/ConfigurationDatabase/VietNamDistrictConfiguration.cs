using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class VietNamDistrictConfiguration : IEntityTypeConfiguration<VietNamDistrict>
    {
        public void Configure(EntityTypeBuilder<VietNamDistrict> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
