using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;
using T.Library.Model.Orders;

namespace T.Library.Model.Orders
{
    public partial class ShoppingCartItemAttributeValue : BaseEntity
    {
        public int ShoppingCartItemId { get; set; }
        /// <summary>
        /// Gets or sets the product attribute identifier
        /// </summary>
        public int ProductAttributeId { get; set; }
        public int ProductAttributeValueId { get; set; }
        public ShoppingCartItem ShoppingCartItem { get; set; }

    }
}
