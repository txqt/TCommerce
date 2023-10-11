using T.Library.Model;

namespace T.Web.Areas.Admin.Models
{
    public class ProductListViewModel
    {
        public List<Product> ProductList { get; set; } = new List<Product>();
        public MetaData MetaData { get; set; } = new MetaData();
        public ProductParameters Parameters { get; set; } = new ProductParameters();
    }
}
