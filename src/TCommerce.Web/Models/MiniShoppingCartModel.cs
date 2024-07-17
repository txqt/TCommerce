using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Web.Models
{
    public class MiniShoppingCartModel
    {
        public MiniShoppingCartModel()
        {
            Items = new List<ShoppingCartItemModel>();
        }

        public IList<ShoppingCartItemModel> Items { get; set; }
        public int TotalProducts { get; set; }
        public string SubTotal { get; set; }
        public decimal SubTotalValue { get; set; }
        public bool DisplayShoppingCartButton { get; set; } = true;
        public bool DisplayCheckoutButton { get; set; } = true;
        public bool ShowProductImages { get; set; }


        public partial class ShoppingCartItemModel : BaseEntity
        {
            public ShoppingCartItemModel()
            {
                Picture = new PictureModel();
            }

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public int Quantity { get; set; }

            public string Price { get; set; }

            //public decimal UnitPriceValue { get; set; }

            public string AttributeInfo { get; set; }

            public PictureModel Picture { get; set; }
        }
    }
}
