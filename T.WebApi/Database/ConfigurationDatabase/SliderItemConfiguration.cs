using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;
using T.Library.Model.BannerItem;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public class SliderItemConfiguration : IEntityTypeConfiguration<SliderItem>
    {
        public void Configure(EntityTypeBuilder<SliderItem> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
