using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Discounts;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class DiscountUsageHistoryConfiguration : IEntityTypeConfiguration<DiscountUsageHistory>
    {
        public void Configure(EntityTypeBuilder<DiscountUsageHistory> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
