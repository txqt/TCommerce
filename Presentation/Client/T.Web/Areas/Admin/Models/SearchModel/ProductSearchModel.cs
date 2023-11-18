using Microsoft.AspNetCore.Mvc.Rendering;

namespace T.Web.Areas.Admin.Models.SearchModel
{
    public class ProductSearchModel : ProductParameters
    {
        public ProductSearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
        }

        public IList<SelectListItem> AvailableCategories { get; set; }
    }
}
