

namespace TCommerce.Web.Areas.Admin.Models.Catalog
{
    public partial record AddProductManufacturerModel
    {
        #region Ctor

        public AddProductManufacturerModel()
        {
            SelectedProductIds = new List<int>();
        }
        #endregion

        #region Properties

        public int ManufacturerId { get; set; }

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}
