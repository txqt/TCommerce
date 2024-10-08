﻿using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Web.Models
{
    public class ProductBoxModel : BaseEntity
    {
        public ProductBoxModel()
        {
            Categories = new List<CategoryOfProduct>();
            ProductPrice = new ProductPriceModel();
            ProductImage = new PictureModel();
        }

        public string ProductName { get; set; }
        public string ProductSeName { get; set; }
        public PictureModel ProductImage { get; set; }
        public bool DisableBuyButton { get; set; }
        public bool DisableWishlistButton { get; set; }
        public List<CategoryOfProduct> Categories { get; set; }
        public ProductPriceModel ProductPrice { get; set; }
        public string ShortDescription { get; set; }
        public class CategoryOfProduct
        {
            public string CategoryName { get; set; }
            public string SeName { get; set; }
        }
        public partial class ProductPriceModel : BaseEntity
        {
            public string OldPrice { get; set; }
            public decimal? OldPriceValue { get; set; }
            public string Price { get; set; }
            public decimal PriceValue { get; set; }
            public string PriceWithDiscount { get; set; }
            public decimal? PriceWithDiscountValue { get; set; }
            public int ProductId { get; set; }
            public bool HidePrices { get; set; }

            //rental
            //public bool IsRental { get; set; }
            //public string RentalPrice { get; set; }
            //public decimal? RentalPriceValue { get; set; }
        }
    }
    
}
