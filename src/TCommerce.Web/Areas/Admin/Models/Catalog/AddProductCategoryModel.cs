

namespace TCommerce.Web.Areas.Admin.Models.Catalog
{
    public partial record AddProductCategoryModel
    {
        #region Ctor

        public AddProductCategoryModel()
        {
            SelectedProductIds = new List<int>();
        }
        #endregion

        #region Properties

        public int CategoryId { get; set; }

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}
