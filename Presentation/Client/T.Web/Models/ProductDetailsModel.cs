using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Common;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;

namespace T.Web.Models
{
    public class ProductDetailsModel
    {
        public ProductDetailsModel()
        {
            ProductAttributes = new List<ProductAttributeModel>();
            ThumbImage = new List<PictureModel>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        public decimal OldPrice { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public PictureModel MainImage { get;set; }

        public List<PictureModel> ThumbImage { get; set; }

        public List<ProductAttributeModel> ProductAttributes { get; set; }

        public int Quantity { get; set; }

        public string SeName { get; set; }

        public List<CategoryOfProduct> Categories { get; set; }

        public CartItemUpdateInfo ItemUpdateInfo { get; set; }
        public AddToCartModel AddToCart { get; set; }
        public class ProductAttributeModel : BaseEntity
        {
            public ProductAttributeModel()
            {
                Values = new List<ProductAttributeValueModel>();
            }

            public int ProductId { get; set; }

            public int ProductAttributeId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }

            /// <summary>
            /// A value indicating whether this attribute depends on some other attribute
            /// </summary>
            public bool HasCondition { get; set; }

            public int AttributeControlTypeId { get; set; }


            public List<ProductAttributeValueModel> Values { get; set; }
        }
        public class ProductAttributeValueModel : BaseEntity
        {
            public ProductAttributeValueModel()
            {
                ImageSquaresPictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string ColorSquaresRgb { get; set; }

            //picture model is used with "image square" attribute type
            public PictureModel ImageSquaresPictureModel { get; set; }

            public string PriceAdjustment { get; set; }

            public bool PriceAdjustmentUsePercentage { get; set; }

            public decimal PriceAdjustmentValue { get; set; }

            public bool IsPreSelected { get; set; }

            //product picture ID (associated to this value)
            public int PictureId { get; set; }

            public bool CustomerEntersQty { get; set; }

            public int Quantity { get; set; }
        }
        public class AddToCartModel : BaseEntity
        {
            public AddToCartModel()
            {
                AllowedQuantities = new List<SelectListItem>();
            }
            public int ProductId { get; set; }
            public int EnteredQuantity { get; set; }
            public string MinimumQuantityNotification { get; set; }
            public List<SelectListItem> AllowedQuantities { get; set; }
            public decimal CustomerEnteredPrice { get; set; }
            public string CustomerEnteredPriceRange { get; set; }

            public bool DisableBuyButton { get; set; }
            public bool DisableWishlistButton { get; set; }

            //pre-order
            public bool AvailableForPreOrder { get; set; }
            public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }
            public string PreOrderAvailabilityStartDateTimeUserTime { get; set; }

            //updating existing shopping cart or wishlist item?
            public int UpdatedShoppingCartItemId { get; set; }
            public ShoppingCartType? UpdateShoppingCartItemType { get; set; }
        }
        public class CategoryOfProduct
        {
            public string CategoryName { get; set; }
            public string SeName { get; set; }
        }
        public class CartItemUpdateInfo
        {
            public string ProductName { get; set; }
            public string AttributeInfo { get; set; }
            public string Quantity { get; set; }
        }
    }
    
}
