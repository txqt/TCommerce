using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TCommerce.Core.Models.Users;

namespace TCommerce.Web.Database.ConfigurationDatabase
{
    public partial class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(30);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(30);
            builder.Property(x => x.PhoneNumber).HasMaxLength(10);
            builder.Property(x => x.Dob).IsRequired();

            builder.HasIndex(x => x.UserName).IsUnique(); 
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.PhoneNumber).IsUnique();
        }
    }
}
