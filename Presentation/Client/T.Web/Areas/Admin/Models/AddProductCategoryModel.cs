using T.Web.Areas.Admin.Models.SearchModel;

namespace T.Web.Areas.Admin.Models
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
