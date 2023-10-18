using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using T.Library.Model;
using T.Library.Model.BannerItem;
using T.Library.Model.Common;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.WebApi.Extensions;

namespace T.WebApi.Database.ConfigurationDatabase
{
    public class DatabaseContext : IdentityDbContext<User, Role, Guid>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {
        }

        public DatabaseContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Bỏ tiền tố AspNet của các bảng: mặc định các bảng trong IdentityDbContext có
            // tên với tiền tố AspNet như: AspNetUserRoles, AspNetUser ...
            // Đoạn mã sau chạy khi khởi tạo DbContext, tạo database sẽ loại bỏ tiền tố đó
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Picture> Picture { get; set; }
        public DbSet<ProductAttribute> ProductAttribute { get; set; }
        public DbSet<ProductAttributeMapping> Product_ProductAttribute_Mapping { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValue { get; set; }
        public DbSet<ProductCategory> Product_ProductCategory_Mapping { get; set; }
        public DbSet<ProductPicture> Product_ProductPicture_Mapping { get; set; }
        public DbSet<ProductReview> ProductReview { get; set; }
        public DbSet<ProductReviewHelpfulness> ProductReviewHelpfulness { get; set; }
        public DbSet<SlideShow> SliderItem { get; set; }
        public DbSet<PermissionRecord> PermissionRecords { get; set; }
        public DbSet<PermissionRecordUserRoleMapping> PermissionRecordUserRoleMappings { get; set; }
    }
}
