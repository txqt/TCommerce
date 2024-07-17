using System.ComponentModel.DataAnnotations;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Discounts;

namespace TCommerce.Web.Areas.Admin.Models
{
    public class DiscountModel : BaseEntity
    {
        public string? Name { get; set; }

        [Display(Name = "Discount Type")]
        public int DiscountTypeId { get; set; }
        public string DiscountTypeName { get; set; }
        public bool UsePercentage { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
        public bool RequiresCouponCode { get; set; }
        public string? CouponCode { get; set; }
        public bool IsCumulative { get; set; }

        [Display(Name = "Discount Limitation")]
        public int DiscountLimitationId { get; set; }
        public int LimitationTimes { get; set; }
        public int? MaximumDiscountedQuantity { get; set; }
        public bool AppliedToSubCategories { get; set; }
        public bool IsActive { get; set; }
        public int TimesUsed { get; set; }
    }
}
