using T.Library.Model.Common;

namespace T.Web.Areas.Admin.Models
{
    public class RelatedProductModel : BaseEntity
    {
        public int ProductId2 { get; set; }
        public string Product2Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
