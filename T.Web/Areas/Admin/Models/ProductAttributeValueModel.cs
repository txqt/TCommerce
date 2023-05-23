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
        }

        #endregion

        #region Properties

        public int ProductAttributeMappingId { get; set; }

        [Display(Name = "Tên giá trị")]
        public string Name { get; set; }

        public string ColorSquaresRgb { get; set; }

        public bool DisplayColorSquaresRgb { get; set; }

        [Display(Name = "Điều chỉnh giá")]

        public decimal PriceAdjustment { get; set; }

        //used only on the values list page
        public string PriceAdjustmentStr { get; set; }

        [Display(Name = "Điều chỉnh giá theo tỷ lệ phần trăm (%)")]
        public bool PriceAdjustmentUsePercentage { get; set; }

        [Display(Name = "Điều chỉnh trọng lượng")]
        public decimal WeightAdjustment { get; set; }

        //used only on the values list page
        public string WeightAdjustmentStr { get; set; }

        public decimal Cost { get; set; }

        public bool CustomerEntersQty { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Được chọn trước ?")]
        public bool IsPreSelected { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; }

        public string PictureThumbnailUrl { get; set; }

        [Display(Name ="Danh sách hình ảnh")]
        public IList<ProductPictureModel> ProductPictureModels { get; set; }

        public int PictureId { get; set; }
        #endregion
    }
}
