using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Configuration;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
