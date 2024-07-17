using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Shippings
{
    public class ShippingMethod : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Fee { get; set; }
        public int Rate { get; set; }
        public int DisplayOrder { get; set; }
        public bool Selected { get; set; }
    }
}
