using T.Library.Model.Common;
using T.Library.Model.ViewsModel;

namespace T.Web.Models
{
    public class HomePageModel : BaseEntity
    {
        public string Title { get; set; }
        public List<ProductBoxModel> ProductList { get; set; }
    }
}
