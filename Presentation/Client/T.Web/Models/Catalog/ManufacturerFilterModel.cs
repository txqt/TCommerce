using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model.Common;

namespace T.Web.Models.Catalog
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
