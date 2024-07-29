using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TCommerce.Web.Areas.Admin.Models.Orders
{
    public class OrderSearchModel
    {
        #region Ctor

        public OrderSearchModel()
        {
            AvailableOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailablePaymentMethods = new List<SelectListItem>();
            OrderStatusIds = new List<int>();
            PaymentStatusIds = new List<int>();
        }

        #endregion

        #region Properties

        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        public List<int> OrderStatusIds { get; set; }

        public List<int> PaymentStatusIds { get; set; }

        public string PaymentMethodSystemName { get; set; }

        public int ProductId { get; set; }

        public string BillingEmail { get; set; }

        public string BillingPhone { get; set; }

        public bool BillingPhoneEnabled { get; set; }

        public string BillingLastName { get; set; }

        public string OrderNotes { get; set; }

        public string GoDirectlyToCustomOrderNumber { get; set; }

        public List<SelectListItem> AvailableOrderStatuses { get; set; }

        public List<SelectListItem> AvailablePaymentStatuses { get; set; }

        public List<SelectListItem> AvailablePaymentMethods { get; set; }

        #endregion
    }
}
