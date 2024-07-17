using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Orders
{
    public partial class ShoppingCartItemAttributeValue : BaseEntity
    {
        public int ShoppingCartItemId { get; set; }
        /// <summary>
        /// Gets or sets the product attribute identifier
        /// </summary>
        public int ProductAttributeId { get; set; }
        public int ProductAttributeValueId { get; set; }
        public ShoppingCartItem? ShoppingCartItem { get; set; }

    }
}
