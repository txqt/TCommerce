using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.ViewsModel
{
    public class ProductUpdateViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string FullDescription { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal OldPrice { get; set; }

        public bool MarkAsNew { get; set; }

        public DateTime MarkAsNewStartDateTimeUtc { get; set; }

        public DateTime MarkAsNewEndDateTimeUtc { get; set; }

        public int DisplayOrder { get; set; }

        public bool Published { get; set; }

        public bool VisibleIndividually { get; set; }

        public string AdminComment { get; set; }

        public bool ShowOnHomepage { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the product allows customer reviews
        /// </summary>
        public bool AllowUserReviews { get; set; }

        /// <summary>
        /// Gets or sets the SKU
        /// </summary>
        public string Sku { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the entity is ship enabled
        /// </summary>
        public bool IsShipEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is free shipping
        /// </summary>
        public bool IsFreeShipping { get; set; }

        /// <summary>
        /// Gets or sets the order minimum quantity
        /// </summary>
        public int OrderMinimumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the order maximum quantity
        /// </summary>
        public int OrderMaximumQuantity { get; set; }

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
    }
}
