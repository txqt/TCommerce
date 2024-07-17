using Microsoft.AspNetCore.Mvc.Rendering;

namespace TCommerce.Web.Areas.Admin.Models.SearchModel
{
    public class DiscountSearchModel
    {
        public DiscountSearchModel()
        {
            AvailableDiscountTypes = new List<SelectListItem>();
            AvailableActiveOptions = new List<SelectListItem>();
        }
        public DateTime? SearchStartDateUtc { get; set; }

        public DateTime? SearchEndDateUtc { get; set; }

        public string? SearchCouponCode { get; set; }

        public string? SearchDiscountName { get; set; }

        public int SearchDiscountTypeId { get; set; }

        public int IsActiveId { get; set; }

        public List<SelectListItem> AvailableDiscountTypes { get;set; }

        public List<SelectListItem> AvailableActiveOptions { get;set; }
    }
}
