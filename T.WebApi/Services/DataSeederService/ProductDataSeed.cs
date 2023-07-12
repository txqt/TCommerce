using T.Library.Model;

namespace T.WebApi.Services.DataSeederService
{
    public class ProductDataSeed
    {
        public List<ProductSeedModel> GetAll()
        {
            return new List<ProductSeedModel>()
            {
                new ProductSeedModel()
                {
                    Products = new List<Product>
                    {
                        new Product
                        {
                            Name = "Áo thun nam",
                            Price = 200000,
                            ShortDescription = "Áo thun nam hàng hiệu",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Áo khoác nữ",
                            Price = 500000,
                            ShortDescription = "Áo khoác dành cho nữ thời trang",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Quần jean nam",
                            Price = 400000,
                            ShortDescription = "Quần jean nam hàng hiệu",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Váy đầm dự tiệc",
                            Price = 1000000,
                            ShortDescription = "Váy đầm dự tiệc sang trọng",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Giày thể thao nam",
                            Price = 800000,
                            ShortDescription = "Giày thể thao nam Adidas",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Giày cao gót nữ",
                            Price = 700000,
                            ShortDescription = "Giày cao gót nữ đẹp",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Túi xách nữ",
                            Price = 600000,
                            ShortDescription = "Túi xách nữ hàng hiệu",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Mũ len nam",
                            Price = 100000,
                            ShortDescription = "Mũ len nam giữ ấm",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Khẩu trang y tế",
                            Price = 5000,
                            ShortDescription = "Khẩu trang y tế 3 lớp",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Balo laptop",
                            Price = 900000,
                            ShortDescription = "Balo laptop chống sốc",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                    },
                    CategoryName = CategoryName.Fashion.ToString(),
                    ProductAttributeName = ProductAttributeName.Color.ToString(),
                    ProductAttributeValues = new List<string>()
                    {
                        "Đỏ", "Xanh", "Vàng", "Lục", "Lam"
                    }
                },
                new ProductSeedModel()
                {
                    Products = new List<Product>()
                    {
                        new Product
                        {
                            Name = "Máy ảnh DSLR",
                            Price = 15000000,
                            ShortDescription = "Máy ảnh Canon EOS 90D",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Loa bluetooth",
                            Price = 2000000,
                            ShortDescription = "Loa bluetooth JBL Flip 5",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Tai nghe true wireless",
                            Price = 5000000,
                            ShortDescription = "Tai nghe true wireless Apple AirPods Pro",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Chuột gaming",
                            Price = 1000000,
                            ShortDescription = "Chuột gaming",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        }
                    },
                    CategoryName = CategoryName.Electronic.ToString(),
                    ProductAttributeName = ProductAttributeName.Color.ToString(),
                    ProductAttributeValues = new List<string>()
                    {
                        "Đỏ", "Xanh", "Vàng", "Lục", "Lam"
                    }
                },
                new ProductSeedModel()
                {
                    Products = new List<Product>()
                    {
                        new Product
                        {
                            Name = "Tủ lạnh",
                            Price = 15000000,
                            ShortDescription = "Tủ lạnh side by side LG",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Máy giặt",
                            Price = 10000000,
                            ShortDescription = "Máy giặt Samsung",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Điều hòa",
                            Price = 8000000,
                            ShortDescription = "Điều hòa Panasonic Inverter",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        },
                        new Product
                        {
                            Name = "Tivi OLED",
                            Price = 30000000,
                            ShortDescription = "Tivi OLED Sony 65 inch",
                            FullDescription = "Full Description",
                            ShowOnHomepage = true,
                            CreatedOnUtc = DateTime.UtcNow,
                        }
                    },
                    CategoryName = CategoryName.Accessory.ToString(),
                    ProductAttributeName = ProductAttributeName.Color.ToString(),
                    ProductAttributeValues = new List<string>()
                    {
                        "Đỏ", "Xanh", "Vàng", "Lục", "Lam"
                    }
                }
            };
        }
    }
}
