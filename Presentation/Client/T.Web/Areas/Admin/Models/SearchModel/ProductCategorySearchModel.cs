using Microsoft.AspNetCore.Mvc.Rendering;

namespace T.Web.Areas.Admin.Models.SearchModel
{
    public class ProductCategorySearchModel : QueryStringParameters
    {
        public ProductCategorySearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
        }

        public int SearchByCategoryId { get; set; }
        public int SearchByManufacturerId { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }
        public List<SelectListItem> AvailableManufacturers{ get; set; }
    }
}
