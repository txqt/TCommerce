using System.ComponentModel.DataAnnotations;

namespace T.Web.Areas.Admin.Models
{
    public class ProductPictureModel
    {
        #region Properties

        public int ProductId { get; set; }

        [UIHint("MultiPicture")]
        public int PictureId { get; set; }

        public string PictureUrl { get; set; }

        public int DisplayOrder { get; set; }

        public string OverrideAltAttribute { get; set; }

        public string OverrideTitleAttribute { get; set; }

        #endregion
    }
}
