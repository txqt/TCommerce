using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Discounts;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class DiscountUsageHistoryConfiguration : IEntityTypeConfiguration<DiscountUsageHistory>
    {
        public void Configure(EntityTypeBuilder<DiscountUsageHistory> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
