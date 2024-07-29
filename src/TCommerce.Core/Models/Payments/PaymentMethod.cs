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
        public bool SupportRefund { get; set; }
        public bool SupportPartiallyRefund { get; set; }
        public bool SupportVoid { get; set; }
    }
}
