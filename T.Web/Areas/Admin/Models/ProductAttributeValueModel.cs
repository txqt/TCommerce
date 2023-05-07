﻿using System.ComponentModel.DataAnnotations;

namespace T.Web.Areas.Admin.Models
{
    public class ProductAttributeValueModel
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

        public int AttributeValueTypeId { get; set; }

        public string AttributeValueTypeName { get; set; }

        public int AssociatedProductId { get; set; }

        public string AssociatedProductName { get; set; }

        public string Name { get; set; }

        public string ColorSquaresRgb { get; set; }

        public bool DisplayColorSquaresRgb { get; set; }

        [UIHint("Picture")]
        public int ImageSquaresPictureId { get; set; }

        public bool DisplayImageSquaresPicture { get; set; }

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

        public IList<int> PictureIds { get; set; }

        public string PictureThumbnailUrl { get; set; }

        public IList<ProductPictureModel> ProductPictureModels { get; set; }


        #endregion
    }
}
