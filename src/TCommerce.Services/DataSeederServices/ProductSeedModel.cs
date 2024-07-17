using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Discounts;

namespace TCommerce.Services.DataSeederServices
{
    public class ProductSeedModel
    {
        public List<Product>? Products { get; set; }
        public List<Category>? Categories { get; set; }
        public List<ProductAttribute>? ProductAttributes { get; set; }
        public List<ProductAttributeValue>? ProductAttributeValues { get; set; }
        public List<Manufacturer>? Manufacturers { get; set; }
        public List<Discount>? Discounts { get; set; }
    }
}
