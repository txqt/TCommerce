using Microsoft.AspNetCore.Mvc.Rendering;

namespace T.Web.Models.Catalog
{
    public class CatalogProductsModel : QueryStringParameters
    {
        #region Properties

        /// <summary>
        /// Get or set a value indicating whether to use standard or AJAX products loading (applicable to 'paging', 'filtering', 'view modes') in catalog
        /// </summary>
        public bool UseAjaxLoading { get; set; }

        /// <summary>
        /// Gets or sets the warning message
        /// </summary>
        public string WarningMessage { get; set; }

        /// <summary>
        /// Gets or sets the message if there are no products to return
        /// </summary>
        public string NoResultMessage { get; set; }

        /// <summary>
        /// Gets or sets the price range filter model
        /// </summary>
        public PriceRangeFilterModel PriceRangeFilter { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer filter model
        /// </summary>
        public ManufacturerFilterModel ManufacturerFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether product sorting is allowed
        /// </summary>
        public bool AllowProductSorting { get; set; }

        /// <summary>
        /// Gets or sets available sort options
        /// </summary>
        public List<SelectListItem> AvailableSortOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to change view mode
        /// </summary>
        public bool AllowProductViewModeChanging { get; set; }

        /// <summary>
        /// Gets or sets available view mode options
        /// </summary>
        public List<SelectListItem> AvailableViewModes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets available page size options
        /// </summary>
        public List<SelectListItem> PageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a order by
        /// </summary>
        public new int? OrderBy { get; set; }

        /// <summary>
        /// Gets or sets a product sorting
        /// </summary>
        public string ViewMode { get; set; }

        /// <summary>
        /// Gets or sets the products
        /// </summary>
        public List<ProductBoxModel> Products { get; set; }

        public MetaData PagingMetaData { get; set; }
        #endregion

        #region Ctor

        public CatalogProductsModel()
        {
            PriceRangeFilter = new PriceRangeFilterModel();
            ManufacturerFilter = new ManufacturerFilterModel();
            AvailableSortOptions = new List<SelectListItem>();
            AvailableViewModes = new List<SelectListItem>();
            PageSizeOptions = new List<SelectListItem>();
            Products = new List<ProductBoxModel>();
        }

        #endregion
    }
}
