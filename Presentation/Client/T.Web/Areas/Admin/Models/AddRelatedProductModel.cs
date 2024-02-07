namespace T.Web.Areas.Admin.Models
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
