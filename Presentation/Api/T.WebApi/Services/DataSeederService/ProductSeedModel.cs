using T.Library.Model;
using T.Library.Model.Catalogs;

namespace T.WebApi.Services.DataSeederService
{
    public class ProductSeedModel
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<ProductAttribute> ProductAttributes { get; set; }
        public List<ProductAttributeValue> ProductAttributeValues { get; set; }
        public List<Manufacturer> Manufacturers { get; set; }
    }
}
