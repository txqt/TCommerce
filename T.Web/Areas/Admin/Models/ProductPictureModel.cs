using System.ComponentModel.DataAnnotations;
using T.Library.Model;

namespace T.Web.Areas.Admin.Models
{
    public class ProductPictureModel : BaseEntity
    {
        #region Properties

        public int ProductId { get; set; }

        [UIHint("MultiPicture")]
        public int PictureId { get; set; }

        public string PictureUrl { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; }

        public string OverrideAltAttribute { get; set; }

        public string OverrideTitleAttribute { get; set; }

        #endregion
    }
}
