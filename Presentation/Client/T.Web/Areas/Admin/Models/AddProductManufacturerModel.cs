using T.Web.Areas.Admin.Models.SearchModel;

namespace T.Web.Areas.Admin.Models
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
