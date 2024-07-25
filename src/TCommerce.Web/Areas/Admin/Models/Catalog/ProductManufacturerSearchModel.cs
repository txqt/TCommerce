using Microsoft.AspNetCore.Mvc.Rendering;

namespace TCommerce.Web.Areas.Admin.Models.Catalog
{
    public class ProductManufacturerSearchModel
    {
        public ProductManufacturerSearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
        }

        public int SearchByCategoryId { get; set; }
        public int SearchByManufacturerId { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }
        public List<SelectListItem> AvailableManufacturers { get; set; }
    }
}
