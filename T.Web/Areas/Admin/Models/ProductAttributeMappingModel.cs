using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using T.Library.Model;

namespace T.Web.Areas.Admin.Models
{
    public class ProductAttributeMappingModel:BaseEntity
    {
        #region Ctor

        public ProductAttributeMappingModel()
        {
            AvailableProductAttributes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public string ProductAttributeName { get; set; }
        public IList<SelectListItem> AvailableProductAttributes { get; set; }
        public string TextPrompt { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
        [UIHint("Int32Nullable")]
        public int? ValidationMinLength { get; set; }
        [UIHint("Int32Nullable")]
        public int? ValidationMaxLength { get; set; }
        public string ValidationFileAllowedExtensions { get; set; }
        [UIHint("Int32Nullable")]
        public int? ValidationFileMaximumSize { get; set; }
        public string DefaultValue { get; set; }

        //public string ValidationRulesString { get; set; }

        #endregion
    }
}
