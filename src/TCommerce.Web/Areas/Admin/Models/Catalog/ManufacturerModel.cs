using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Areas.Admin.Models.Catalog
{
    public class ManufacturerModel : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        /// <summary>
        /// Gets or sets the parent picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

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

        public int PageSize { get; set; } = 10;

        public string PageSizeOptions { get; set; }

        public string SeName { get; set; }
    }
}
