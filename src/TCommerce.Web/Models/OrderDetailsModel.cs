using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Web.Models
{
    public class OrderDetailsModel : BaseEntity
    {
        public OrderDetailsModel()
        {
            Items = new List<OrderItemModel>();
            OrderNotes = new List<OrderNote>();

            ShippingAddress = new AddressModel();
        }

        public DateTime CreatedOn { get; set; }

        public string OrderStatus { get; set; }

        public bool IsReOrderAllowed { get; set; }
        public string ShippingStatus { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public string ShippingMethod { get; set; }

        public string PaymentMethod { get; set; }

        public string OrderSubtotal { get; set; }
        public decimal OrderSubtotalValue { get; set; }
        public string OrderSubTotalDiscount { get; set; }
        public decimal OrderSubTotalDiscountValue { get; set; }
        public string OrderShipping { get; set; }
        public decimal OrderShippingValue { get; set; }

        public bool PricesIncludeTax { get; set; }

        public string OrderTotalDiscount { get; set; }
        public decimal OrderTotalDiscountValue { get; set; }
        public string OrderTotal { get; set; }
        public decimal OrderTotalValue { get; set; }

        public bool ShowSku { get; set; }
        public IList<OrderItemModel> Items { get; set; }

        public IList<OrderNote> OrderNotes { get; set; }
        public bool ShowProductThumbnail { get; set; }

        public bool PrintMode { get; set; }
        public bool PdfInvoiceDisabled { get; set; }

        #region Nested Classes

        public class OrderItemModel : BaseEntity
        {
            public OrderItemModel()
            {
                Picture = new PictureModel();
            }

            public Guid OrderItemGuid { get; set; }
            public string Sku { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string Price { get; set; }
            public decimal PriceValue { get; set; }
            public string SubTotal { get; set; }
            public decimal SubTotalValue { get; set; }
            public int Quantity { get; set; }
            public PictureModel Picture { get; set; }
            public string AttributeInfo { get; set; }
        }

        public class OrderNote : BaseEntity
        {
            public string Note { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }
}
