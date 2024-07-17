using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Discounts;

namespace TCommerce.Core.Models.Paging
{
    public class DiscountParameters : QueryStringParameters
    {
        public List<int>? ids { get; set; } = null;

        public DateTime? StartDateUtc { get; set; }

        public DateTime? EndDateUtc { get; set; }

        public string? CouponCode { get; set; }

        public string? DiscountName { get; set; }

        public int IsActiveId { get; set; }

        public DiscountType DiscountType { get; set; }
    }
}
