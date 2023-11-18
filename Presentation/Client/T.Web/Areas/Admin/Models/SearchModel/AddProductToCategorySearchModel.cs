using Microsoft.AspNetCore.Mvc.Rendering;

namespace T.Web.Areas.Admin.Models.SearchModel
{
    public class AddProductToCategorySearchModel
    {
        public AddProductToCategorySearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
        }

        public int SearchByCategoryId { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
    }
}
