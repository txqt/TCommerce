using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T.Library.Model.Orders;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public partial class ShoppingCartItemConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.ShoppingCartItemAttributeValues).WithOne(x => x.ShoppingCartItem).HasForeignKey(x => x.ShoppingCartItemId);
        }
    }
}
