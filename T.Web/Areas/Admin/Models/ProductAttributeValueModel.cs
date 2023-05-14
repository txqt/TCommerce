using System.ComponentModel.DataAnnotations;
using T.Library.Model;

namespace T.Web.Areas.Admin.Models
{
    public class ProductAttributeValueModel : BaseEntity
    {
        #region Ctor

        public ProductAttributeValueModel()
        {
            ProductPictureModels = new List<ProductPictureModel>();
            PictureIds = new List<int>();
        }

        #endregion

        #region Properties

        public int ProductAttributeMappingId { get; set; }

        public string Name { get; set; }

        public string ColorSquaresRgb { get; set; }

        public bool DisplayColorSquaresRgb { get; set; }

        public decimal PriceAdjustment { get; set; }

        //used only on the values list page
        public string PriceAdjustmentStr { get; set; }

        public bool PriceAdjustmentUsePercentage { get; set; }

        public decimal WeightAdjustment { get; set; }

        //used only on the values list page
        public string WeightAdjustmentStr { get; set; }

        public decimal Cost { get; set; }

        public bool CustomerEntersQty { get; set; }

        public int Quantity { get; set; }

        public bool IsPreSelected { get; set; }

        public int DisplayOrder { get; set; }
        [Display(Name = "Danh sách hình ảnh")]
        public IList<int> PictureIds { get; set; }

        public string PictureThumbnailUrl { get; set; }

        public IList<ProductPictureModel> ProductPictureModels { get; set; }

        public int PictureId { get; set; }
        #endregion
    }
}
