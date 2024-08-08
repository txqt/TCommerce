using System.ComponentModel.DataAnnotations;
using TCommerce.Core.Models.Common;
using TCommerce.Web.Areas.Admin.Models.Datatables;

namespace TCommerce.Web.Areas.Admin.Models.Catalog
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

        #endregion
    }
}
