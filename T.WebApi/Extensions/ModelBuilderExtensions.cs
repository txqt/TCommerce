using Microsoft.EntityFrameworkCore;
using T.Library.Model;

namespace T.WebApi.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            #region Product Seed
            modelBuilder.Entity<Product>().HasData(
              new Product
              {
                  Id = 1,
                  Name = "Áo thun nam",
                  Price = 200000,
                  ShortDescription = "Áo thun nam hàng hiệu",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 2,
                  Name = "Áo khoác nữ",
                  Price = 500000,
                  ShortDescription = "Áo khoác dành cho nữ thời trang",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 3,
                  Name = "Quần jean nam",
                  Price = 400000,
                  ShortDescription = "Quần jean nam hàng hiệu",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 4,
                  Name = "Váy đầm dự tiệc",
                  Price = 1000000,
                  ShortDescription = "Váy đầm dự tiệc sang trọng",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 5,
                  Name = "Giày thể thao nam",
                  Price = 800000,
                  ShortDescription = "Giày thể thao nam Adidas",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 6,
                  Name = "Giày cao gót nữ",
                  Price = 700000,
                  ShortDescription = "Giày cao gót nữ đẹp",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 7,
                  Name = "Túi xách nữ",
                  Price = 600000,
                  ShortDescription = "Túi xách nữ hàng hiệu",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 8,
                  Name = "Mũ len nam",
                  Price = 100000,
                  ShortDescription = "Mũ len nam giữ ấm",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 9,
                  Name = "Khẩu trang y tế",
                  Price = 5000,
                  ShortDescription = "Khẩu trang y tế 3 lớp",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 10,
                  Name = "Balo laptop",
                  Price = 900000,
                  ShortDescription = "Balo laptop chống sốc",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 11,
                  Name = "Tủ lạnh",
                  Price = 15000000,
                  ShortDescription = "Tủ lạnh side by side LG",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 12,
                  Name = "Máy giặt",
                  Price = 10000000,
                  ShortDescription = "Máy giặt Samsung",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 13,
                  Name = "Điều hòa",
                  Price = 8000000,
                  ShortDescription = "Điều hòa Panasonic Inverter",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 14,
                  Name = "Tivi OLED",
                  Price = 30000000,
                  ShortDescription = "Tivi OLED Sony 65 inch",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 15,
                  Name = "Máy ảnh DSLR",
                  Price = 15000000,
                  ShortDescription = "Máy ảnh Canon EOS 90D",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 16,
                  Name = "Loa bluetooth",
                  Price = 2000000,
                  ShortDescription = "Loa bluetooth JBL Flip 5",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 17,
                  Name = "Tai nghe true wireless",
                  Price = 5000000,
                  ShortDescription = "Tai nghe true wireless Apple AirPods Pro",
                  FullDescription = "Full Description"
              },
              new Product
              {
                  Id = 18,
                  Name = "Chuột gaming",
                  Price = 1000000,
                  ShortDescription = "Chuột gaming",
                  FullDescription = "Full Description"
              }
            );
            #endregion

            #region Category Seed
            modelBuilder.Entity<Category>().HasData(
              new Category
              {
                  Id = 1,
                  Name = "Thời trang",
                  CreatedOnUtc = DateTime.UtcNow,
              },
              new Category
              {
                  Id = 2,
                  Name = "Điện tử",
                  CreatedOnUtc = DateTime.UtcNow,
              },
              new Category
              {
                  Id = 3,
                  Name = "Điện gia dụng",
                  CreatedOnUtc = DateTime.UtcNow,
              },
              new Category
              {
                  Id = 4,
                  Name = "Âm thanh",
                  CreatedOnUtc = DateTime.UtcNow,
              },
              new Category
              {
                  Id = 5,
                  Name = "Phụ kiện",
                  CreatedOnUtc = DateTime.UtcNow,
              }
            );
            #endregion

            #region ProductCategory Seed
            modelBuilder.Entity<ProductCategory>().HasData(

              new ProductCategory
              {
                  Id = 1,
                  ProductId = 1,
                  CategoryId = 1
              },
              new ProductCategory
              {
                  Id = 2,
                  ProductId = 2,
                  CategoryId = 1
              },
              new ProductCategory
              {
                  Id = 3,
                  ProductId = 3,
                  CategoryId = 1
              },
              new ProductCategory
              {
                  Id = 4,
                  ProductId = 4,
                  CategoryId = 1
              },
              new ProductCategory
              {
                  Id = 5,
                  ProductId = 5,
                  CategoryId = 1
              },
              new ProductCategory
              {
                  Id = 6,
                  ProductId = 6,
                  CategoryId = 1
              },
              new ProductCategory
              {
                  Id = 7,
                  ProductId = 7,
                  CategoryId = 1
              },
              new ProductCategory
              {
                  Id = 8,
                  ProductId = 8,
                  CategoryId = 1
              },
              new ProductCategory
              {
                  Id = 9,
                  ProductId = 9,
                  CategoryId = 1
              },
              new ProductCategory
              {
                  Id = 10,
                  ProductId = 10,
                  CategoryId = 1
              },
              new ProductCategory
              {
                  Id = 11,
                  ProductId = 11,
                  CategoryId = 2
              },
              new ProductCategory
              {
                  Id = 12,
                  ProductId = 12,
                  CategoryId = 3
              },
              new ProductCategory
              {
                  Id = 13,
                  ProductId = 13,
                  CategoryId = 3
              },
              new ProductCategory
              {
                  Id = 14,
                  ProductId = 14,
                  CategoryId = 2
              },
              new ProductCategory
              {
                  Id = 15,
                  ProductId = 15,
                  CategoryId = 2
              },
              new ProductCategory
              {
                  Id = 16,
                  ProductId = 16,
                  CategoryId = 4
              },
              new ProductCategory
              {
                  Id = 17,
                  ProductId = 17,
                  CategoryId = 4
              },
              new ProductCategory
              {
                  Id = 18,
                  ProductId = 18,
                  CategoryId = 2
              }
            );
            #endregion

            #region ProductAttribute Seed
            modelBuilder.Entity<ProductAttribute>().HasData(
                    new ProductAttribute()
                    {
                        Id = 1,
                        Name = "Màu sắc",
                        Description = "Thuộc tính màu sắc của sản phẩm"
                    }
                );
            #endregion

            #region ProductAttributeMapping
            modelBuilder.Entity<ProductAttributeMapping>().HasData(
                new ProductAttributeMapping()
                {
                    Id = 1,
                    ProductId = 1,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 2,
                    ProductId = 2,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 3,
                    ProductId = 3,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 4,
                    ProductId = 4,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 5,
                    ProductId = 5,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 6,
                    ProductId = 6,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 7,
                    ProductId = 7,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 8,
                    ProductId = 8,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 9,
                    ProductId = 9,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 10,
                    ProductId = 10,
                    ProductAttributeId = 1,
                }
                ,
                new ProductAttributeMapping()
                {
                    Id = 11,
                    ProductId = 11,
                    ProductAttributeId = 1,
                }
                ,
                new ProductAttributeMapping()
                {
                    Id = 12,
                    ProductId = 12,
                    ProductAttributeId = 1,
                }
                ,
                new ProductAttributeMapping()
                {
                    Id = 13,
                    ProductId = 13,
                    ProductAttributeId = 1,
                }
                ,
                new ProductAttributeMapping()
                {
                    Id = 14,
                    ProductId = 14,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 15,
                    ProductId = 15,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 16,
                    ProductId = 16,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 17,
                    ProductId = 17,
                    ProductAttributeId = 1,
                },
                new ProductAttributeMapping()
                {
                    Id = 18,
                    ProductId = 18,
                    ProductAttributeId = 1,
                }
            );
            #endregion

            #region ProductAttributeValue
            modelBuilder.Entity<ProductAttributeValue>().HasData(
                    new ProductAttributeValue() { 
                        Id = 1, 
                        ProductAttributeMappingId = 1,
                        Name = "Đỏ"
                    },
                    new ProductAttributeValue()
                    {
                        Id = 2,
                        ProductAttributeMappingId = 1,
                        Name = "Xanh"
                    }
                    ,
                    new ProductAttributeValue()
                    {
                        Id = 3,
                        ProductAttributeMappingId = 1,
                        Name = "Vàng"
                    },
                    new ProductAttributeValue()
                    {
                        Id = 4,
                        ProductAttributeMappingId = 2,
                        Name = "Tím"
                    },
                    new ProductAttributeValue()
                    {
                        Id = 5,
                        ProductAttributeMappingId = 2,
                        Name = "Đen"
                    }
                    ,
                    new ProductAttributeValue()
                    {
                        Id = 6,
                        ProductAttributeMappingId = 2,
                        Name = "Vàng"
                    }
                );
            #endregion
        }
    }
}
