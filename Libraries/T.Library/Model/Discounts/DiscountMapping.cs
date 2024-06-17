using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.Discounts
{
    public class DiscountMapping : BaseEntity
    {
        public int DiscountId { get;set; }
        public int EntityId { get;set; }
        public int DiscountTypeId { get; set; }

        [NotMapped]
        public DiscountType DiscountType
        {
            get => (DiscountType)DiscountTypeId;
            set => DiscountTypeId = (int)value;
        }
    }
}
