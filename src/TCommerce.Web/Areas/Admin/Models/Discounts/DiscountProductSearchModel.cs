using Microsoft.AspNetCore.Mvc.Rendering;

namespace TCommerce.Web.Areas.Admin.Models.Discounts
{
    public class DiscountProductSearchModel
    {
        public DiscountProductSearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
        }
        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }
        public List<SelectListItem> AvailableCategories { get; set; }
        public List<SelectListItem> AvailableManufacturers { get; set; }
    }
}
