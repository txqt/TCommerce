namespace TCommerce.Web.Areas.Admin.Models.Catalog
{
    public class AddRelatedProductModel
    {
        #region Ctor

        public AddRelatedProductModel()
        {
            SelectedProductIds = new List<int>();
        }
        #endregion

        #region Properties

        public int ProductId { get; set; }

        public List<int> SelectedProductIds { get; set; }

        #endregion
    }
}
