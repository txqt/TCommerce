using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.ViewsModel
{
    public class ProductModel
    {
        public int Id { get; set; }
        [Required, Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }
        [Required, Display(Name = "Mổ tả ngắn")]
        public string ShortDescription { get; set; }
        [Required, Display(Name = "Mô tả đầy đủ")]
        public string FullDescription { get; set; }
        [Required, Display(Name = "Số lượng")]
        public int StockQuantity { get; set; }
        [Required, Display(Name = "Giá")]
        public decimal Price { get; set; }
        [Required, Display(Name = "Giá cũ")]
        public decimal OldPrice { get; set; }
        [Display(Name = "Đánh dấu là sản phẩm mới")]
        public bool MarkAsNew { get; set; }
        [Display(Name = "Thời gian bắt đầu đánh dấu sản phẩm mới")]
        public DateTime MarkAsNewStartDateTimeUtc { get; set; }
        [Display(Name = "Thời gian kết thúc đánh dấu sản phẩm mới")]
        public DateTime MarkAsNewEndDateTimeUtc { get; set; }
        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; }

        public bool Published { get; set; }

        public bool VisibleIndividually { get; set; }

        public string AdminComment { get; set; }
        [Display(Name = "Hiện thị ở trang chủ")]
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
        public List<ProductAttributeMapping> AttributeMappings { get; set; }
    }
}
