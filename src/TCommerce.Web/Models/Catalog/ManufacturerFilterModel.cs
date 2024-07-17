using Microsoft.AspNetCore.Mvc.Rendering;
using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Models.Catalog
{
    public partial class ManufacturerFilterModel : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether filtering is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the filtrable manufacturers
        /// </summary>
        public IList<SelectListItem> Manufacturers { get; set; }

        #endregion

        #region Ctor

        public ManufacturerFilterModel()
        {
            Manufacturers = new List<SelectListItem>();
        }

        #endregion
    }
}
