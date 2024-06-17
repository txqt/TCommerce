using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Banners;
using T.Library.Model.Discounts;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
