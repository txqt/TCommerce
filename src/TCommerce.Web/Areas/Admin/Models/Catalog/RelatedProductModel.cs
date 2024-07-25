using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Areas.Admin.Models.Catalog
{
    public class RelatedProductModel : BaseEntity
    {
        public int ProductId2 { get; set; }
        public string Product2Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
