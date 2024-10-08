﻿using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Web.Models
{
    public class ShoppingCartModel
    {
        public ShoppingCartModel()
        {
            Items = new List<ShoppingCartItemModel>();
            UsedDiscounts = new List<UsedDiscountModel>();
        }

        public IList<ShoppingCartItemModel> Items { get; set; }
        public int TotalProducts { get; set; }
        public bool DisplayShoppingCartButton { get; set; } = true;
        public bool DisplayCheckoutButton { get; set; } = true;
        public bool ShowProductImages { get; set; }
        public List<string> Warnings { get; set; }
        public List<UsedDiscountModel> UsedDiscounts { get; set; }
        public partial class ShoppingCartItemModel : BaseEntity
        {
            public ShoppingCartItemModel()
            {
                Warnings = new List<string>();
                Picture = new PictureModel();
            }

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public int Quantity { get; set; }

            public string Price { get; set; }

            public decimal PriceValue { get; set; }

            public int OrderMinimumQuantity { get; set; }

            public int OrderMaximumQuantity { get; set; }

            public string SubTotal { get; set; }

            public decimal SubTotalValue { get; set; }

            public string AttributeInfo { get; set; }

            public PictureModel Picture { get; set; }

            public List<string> Warnings { get; set; }
        }

        #region Nested Classes

        public class UsedDiscountModel : BaseEntity
        {
            public int DiscountId { get; set; }
            public string DiscountCode { get; set; }
            public string DiscountName { get; set; }
        }

        #endregion
    }
}
