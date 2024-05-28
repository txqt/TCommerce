using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.Paging
{
    public class ShoppingCartParameters : QueryStringParameters
    {
        public int ProductId { get; set; }
        public ShoppingCartType ShoppingCartType { get; set; }

    }
}
