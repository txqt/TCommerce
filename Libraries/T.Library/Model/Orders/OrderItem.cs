using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.Orders
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal? ItemWeight { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalProductCost { get; set; }
        public string? AttributesJson { get; set; }
    }
}
