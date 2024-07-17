using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Orders;

namespace TCommerce.Core.Models.Paging
{
    public class ShoppingCartParameters : QueryStringParameters
    {
        public int ProductId { get; set; }
        public ShoppingCartType ShoppingCartType { get; set; }

    }
}
