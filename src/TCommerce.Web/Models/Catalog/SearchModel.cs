using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Models.Catalog
{
    public class SearchModel : BaseEntity
    {
        public SearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            CatalogProductsModel = new CatalogProductsModel();
        }

        /// <summary>
        /// Query string
        /// </summary>
        [DisplayName("SearchTerm")]
        public string q { get; set; }

        /// <summary>
        /// Category ID
        /// </summary>
        [DisplayName("Category")]
        public int cid { get; set; }

        [DisplayName("IncludeSubCategories")]
        public bool isc { get; set; }

        /// <summary>
        /// Manufacturer ID
        /// </summary>
        [DisplayName("Manufacturer")]
        public int mid { get; set; }

        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        [DisplayName("SearchInDescriptions")]
        public bool sid { get; set; }

        /// <summary>
        /// A value indicating whether "advanced search" is enabled
        /// </summary>
        [DisplayName("AdvancedSearch")]
        public bool advs { get; set; }

        public CatalogProductsModel CatalogProductsModel { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }
        public List<SelectListItem> AvailableManufacturers { get; set; }
        public List<SelectListItem> AvailableVendors { get; set; }
    }
}
