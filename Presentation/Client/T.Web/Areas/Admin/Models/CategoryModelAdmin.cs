using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model.ViewsModel;

namespace T.Web.Areas.Admin.Models
{
    public class CategoryModelAdmin : CategoryModel
    {
        public CategoryModelAdmin()
        {
            AvailableCategories = new List<SelectListItem>();
        }

        public List<SelectListItem> AvailableCategories { get; set; }
    }
}
