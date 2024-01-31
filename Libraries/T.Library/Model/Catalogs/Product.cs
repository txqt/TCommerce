using System.ComponentModel.DataAnnotations;
using T.Library.Model.Catalogs;
using T.Library.Model.Common;

namespace T.Library.Model
{
    /// <summary>
    /// Represents a product
    /// </summary>
    public partial class Product : BaseEntity, ISoftDeletedEntity
    {
        ///// <summary>
        ///// Gets or sets the product type identifier
        ///// </summary>
        //public int ProductTypeId { get; set; }

        /// <summary>
        /// Gets or sets the values indicating whether this product is visible in catalog or search results.
        /// It's used when this product is associated to some "grouped" one
        /// This way associated products could be accessed/added/etc only from a grouped product details page
        /// </summary>
        public bool VisibleIndividually { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        public string? ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        public string? FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string? AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a vendor identifier
        /// </summary>
        //public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the product on home page
        /// </summary>
        public bool ShowOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string? MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string? MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string? MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product allows customer reviews
        /// </summary>
        public bool AllowUserReviews { get; set; }

        /// <summary>
        /// Gets or sets the SKU
        /// </summary>
        public string? Sku { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the product is gift card
        /// </summary>
        //public bool IsGiftCard { get; set; }

        /// <summary>
        /// Gets or sets the gift card type identifier
        /// </summary>
        //public int GiftCardTypeId { get; set; }

        /// <summary>
        /// Gets or sets gift card amount that can be used after purchase. If not specified, then product price will be used.
        /// </summary>
        //public decimal? OverriddenGiftCardAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is ship enabled
        /// </summary>
        public bool IsShipEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is free shipping
        /// </summary>
        public bool IsFreeShipping { get; set; }

        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the order minimum quantity
        /// </summary>
        public int OrderMinimumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the order maximum quantity
        /// </summary>
        public int OrderMaximumQuantity { get; set; } = 10;

        /// <summary>
        /// Gets or sets a value indicating whether this product is returnable (a customer is allowed to submit return request with this product)
        /// </summary>
        public bool NotReturnable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable buy (Add to cart) button
        /// </summary>
        public bool DisableBuyButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable "Add to wishlist" button
        /// </summary>
        public bool DisableWishlistButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is available for Pre-Order
        /// </summary>
        public bool AvailableForPreOrder { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the product availability (for pre-order products)
        /// </summary>
        public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the old price
        /// </summary>
        //[Required]
        public decimal OldPrice { get; set; }

        public bool MarkAsNew { get; set; } = false;

        /// <summary>
        /// Gets or sets the start date and time of the new product (set product as "New" from date). Leave empty to ignore this property
        /// </summary>
        public DateTime? MarkAsNewStartDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the end date and time of the new product (set product as "New" to date). Leave empty to ignore this property
        /// </summary>
        public DateTime? MarkAsNewEndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this product has tier prices configured
        /// <remarks>The same as if we run TierPrices.Count > 0
        /// We use this property for performance optimization:
        /// if this property is set to false, then we do not need to load tier prices navigation property
        /// </remarks>
        /// </summary>
        //public bool HasTierPrices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this product has discounts applied
        /// <remarks>The same as if we run AppliedDiscounts.Count > 0
        /// We use this property for performance optimization:
        /// if this property is set to false, then we do not need to load Applied Discounts navigation property
        /// </remarks>
        /// </summary>
        //public bool HasDiscountsApplied { get; set; }

        /// <summary>
        /// Gets or sets the weight
        /// </summary>
        public decimal Weight { get; set; } = 0;

        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public decimal Length { get; set; } = 0;

        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public decimal Width { get; set; } = 0;

        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public decimal Height { get; set; } = 0;

        /// <summary>
        /// Gets or sets the available start date and time
        /// </summary>
        public DateTime? AvailableStartDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the available end date and time
        /// </summary>
        public DateTime? AvailableEndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets a display order.
        /// This value is used when sorting associated products (used with "grouped" products)
        /// This value is used when sorting home page products
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; } = true;

        /// <summary>
        /// Gets or sets the date and time of product creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of product update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }
        public bool Deleted { get; set; }

        ///// <summary>
        ///// Gets or sets the product type
        ///// </summary>
        //public ProductType ProductType
        //{
        //    get => (ProductType)ProductTypeId;
        //    set => ProductTypeId = (int)value;
        //}


        /// <summary>
        /// Gets or sets the gift card type
        /// </summary>
        //public GiftCardType GiftCardType
        //{
        //    get => (GiftCardType)GiftCardTypeId;
        //    set => GiftCardTypeId = (int)value;
        //}

        public List<ProductCategory>? ProductCategories { get; set; }

        public List<ProductAttributeMapping>? AttributeMappings { get; set; }

        public List<ProductPicture>? ProductPictures { get; set; }

        public List<ProductReview>? ProductReviews { get; set; }

        public List<ProductManufacturer>? ProductManufacturers { get; set; }
    }
}