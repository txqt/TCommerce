using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using T.Library.Model.Common;

namespace T.Web.Areas.Admin.Models
{
    public class CategoryModel : BaseEntity
    {
        public CategoryModel()
        {
            AvailableCategories = new List<SelectListItem>();
        }

        [Display(Name = "Tên thể loại")]
        public string? Name { get; set; }

        public string? ParentCategoryName { get; set; }
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

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
        /// Gets or sets the parent category identifier
        /// </summary>
        public int ParentCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the category on home page
        /// </summary>
        [Display(Name = "Hiển thị trên trang chủ ?")]
        public bool ShowOnHomepage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include this category in the top menu
        /// </summary>
        [Display(Name = "Đưa danh mục này vào menu trên cùng ?")]
        public bool IncludeInTopMenu { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; } = true;

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the price range filtering is enabled
        /// </summary>
        public bool PriceRangeFiltering { get; set; }

        /// <summary>
        /// Gets or sets the "from" price
        /// </summary>
        public decimal PriceFrom { get; set; }

        /// <summary>
        /// Gets or sets the "to" price
        /// </summary>
        public decimal PriceTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the price range should be entered manually
        /// </summary>
        public bool ManuallyPriceRange { get; set; }

        public bool AllowCustomersToSelectPageSize { get; set; } = true;

        public int PageSize { get; set; } = 10;

        public string? PageSizeOptions { get; set; }

        public string? SeName { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }
    }
}
