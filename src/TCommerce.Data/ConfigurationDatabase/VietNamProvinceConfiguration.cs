using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class VietNamProvinceConfiguration : IEntityTypeConfiguration<VietNamProvince>
    {
        public void Configure(EntityTypeBuilder<VietNamProvince> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
