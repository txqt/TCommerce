using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Paging
{
    public class OrderParameters : QueryStringParameters
    {
        public Guid? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? OrderNotes { get;set; }
        public List<int>? osIds { get; set; } = null; //order status id list
        public List<int>? psIds { get; set; } = null; //payment status id list
        public string? BillingPhone { get; set; }
        public string? BillingEmail { get; set; }
        public string? BillingLastName { get; set; }
        public string? PaymentMethodSystemName { get; set; }
        public int ProductId { get; set; }
    }
}
