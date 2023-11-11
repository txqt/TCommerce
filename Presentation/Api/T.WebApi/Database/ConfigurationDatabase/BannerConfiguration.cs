using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;
using T.Library.Model.Banners;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class BannerConfiguration : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.Picture).WithMany(x=>x.Banners).HasForeignKey(x=>x.PictureId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
