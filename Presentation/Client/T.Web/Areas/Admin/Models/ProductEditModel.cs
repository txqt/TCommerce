using T.Library.Model.ViewsModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace T.Web.Areas.Admin.Models
{
    public class ProductEditModel : ProductModel
    {
        public ProductEditModel()
        {
            AvailableCategories = new List<SelectListItem>();
        }

        public List<SelectListItem> AvailableCategories { get; set; }
    }
}
