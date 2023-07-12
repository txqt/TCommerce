using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;
using T.Library.Model.BannerItem;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public class SliderItemConfiguration : IEntityTypeConfiguration<SlideShow>
    {
        public void Configure(EntityTypeBuilder<SlideShow> builder)
        {
            builder.HasKey(x => x.Id);
            //builder.HasOne(x => x.Picture).WithMany(x => x.SliderItems).HasForeignKey(x => x.PictureId);
        }
    }
}
