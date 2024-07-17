using TCommerce.Core.Models.Catalogs;

namespace TCommerce.Web.Areas.Admin.Models
{
    public class ProductListViewModel
    {
        public List<Product> ProductList { get; set; } = new List<Product>();
        public MetaData MetaData { get; set; } = new MetaData();
        public ProductParameters Parameters { get; set; } = new ProductParameters();
        public Product ProductModel { get; set; } = new Product();
    }
}
