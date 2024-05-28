using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.Orders
{
    public class Order : BaseEntity, ISoftDeletedEntity
    {
        public Guid UserId { get; set; }
        public bool Deleted { get; set; }
        public int OrderStatusId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public OrderStatus OrderStatus
        {
            get => (OrderStatus)OrderStatusId;
            set => OrderStatusId = (int)value;
        }
    }
}
