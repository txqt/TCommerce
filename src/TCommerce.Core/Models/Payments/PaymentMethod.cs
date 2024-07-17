using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Payments
{
    public class PaymentMethod : BaseEntity
    {
        public string PaymentMethodSystemName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Fee { get; set; }
        public bool Selected { get; set; }
        public string? LogoUrl { get; set; }
    }
}
