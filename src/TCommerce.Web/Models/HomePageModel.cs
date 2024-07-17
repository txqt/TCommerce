using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Models
{
    public class HomePageModel : BaseEntity
    {
        public string Title { get; set; }
        public List<ProductBoxModel> ProductList { get; set; }
    }
}
