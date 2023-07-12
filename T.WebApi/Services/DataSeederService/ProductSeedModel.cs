using T.Library.Model;

namespace T.WebApi.Services.DataSeederService
{
    public class ProductSeedModel
    {
        public List<Product> Products { get; set; }
        public string CategoryName { get; set; }
        public string ProductAttributeName { get; set; }
        public List<string> ProductAttributeValues { get; set; }
    }
}
