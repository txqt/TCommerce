using TCommerce.Web.Areas.Admin.Models.SearchModel;

namespace TCommerce.Web.Areas.Admin.Models
{
    public partial record AddProductDiscountModel
    {
        #region Ctor

        public AddProductDiscountModel()
        {
            SelectedProductIds = new List<int>();
        }
        #endregion

        #region Properties

        public int DiscountId { get; set; }

        public IList<int> SelectedProductIds { get; set; }

        #endregion
    }
}
