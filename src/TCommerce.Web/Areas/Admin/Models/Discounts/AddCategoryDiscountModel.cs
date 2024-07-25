

namespace TCommerce.Web.Areas.Admin.Models.Discounts
{
    public partial record AddCategoryDiscountModel
    {
        #region Ctor

        public AddCategoryDiscountModel()
        {
            SelectedCategoryIds = new List<int>();
        }
        #endregion

        #region Properties

        public int DiscountId { get; set; }

        public IList<int> SelectedCategoryIds { get; set; }

        #endregion
    }
}
