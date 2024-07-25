

namespace TCommerce.Web.Areas.Admin.Models.Discounts
{
    public partial record AddManufacturerDiscountModel
    {
        #region Ctor

        public AddManufacturerDiscountModel()
        {
            SelectedManufacturerIds = new List<int>();
        }
        #endregion

        #region Properties

        public int DiscountId { get; set; }

        public IList<int> SelectedManufacturerIds { get; set; }

        #endregion
    }
}
