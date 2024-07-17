using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Discounts
{
    public partial class DiscountCategoryMapping : DiscountMapping
    {
        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public override int EntityId { get; set; }
    }
}
