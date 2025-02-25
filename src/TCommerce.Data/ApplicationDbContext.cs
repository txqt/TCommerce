﻿using System.Reflection;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.Banners;
using TCommerce.Core.Models.Seo;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Configuration;

namespace TCommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName is not null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                {
                    property.SetColumnType("decimal(18,2)");
                }

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeMapping> Product_ProductAttribute_Mapping { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<ProductCategory> Product_Category_Mapping { get; set; }
        public DbSet<ProductPicture> Product_Picture_Mapping { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductReviewHelpfulness> ProductReviewHelpfulness { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<PermissionRecord> PermissionRecords { get; set; }
        public DbSet<PermissionRecordUserRoleMapping> PermissionRecordUserRoleMappings { get; set; }
        public DbSet<UrlRecord> UrlRecords { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<RelatedProduct> RelatedProducts { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<UserAddressMapping> UserAddressMappings { get; set; }
        public DbSet<VietNamProvince> VietNamProvinces { get; set; }
        public DbSet<VietNamDistrict> VietNamDistricts { get; set; }
        public DbSet<VietNamCommune> VietNamCommunes { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountProductMapping> DiscountProductMappings { get; set; }
        public DbSet<DiscountCategoryMapping> DiscountCategoryMappings { get; set; }
        public DbSet<DiscountManufacturerMapping> DiscountManufacturerMappings { get; set; }
        public DbSet<DiscountUsageHistory> DiscountUsageHistorys { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<OrderNote> OrderNotes { get; set; }
    }
}
