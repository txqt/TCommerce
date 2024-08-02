using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Banners;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Orders;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class OrderNoteConfiguration : IEntityTypeConfiguration<OrderNote>
    {
        public void Configure(EntityTypeBuilder<OrderNote> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
