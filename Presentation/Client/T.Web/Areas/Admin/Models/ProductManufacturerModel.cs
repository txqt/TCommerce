using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using T.Library.Model.Common;

namespace T.Web.Areas.Admin.Models
{
    public class ProductManufacturerModel : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int ManufacturerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is featured
        /// </summary>
        [Display(Name = "Sản phẩm nổi bật ?")]
        public bool IsFeaturedProduct { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; }

        public string ProductName { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
    }
}
