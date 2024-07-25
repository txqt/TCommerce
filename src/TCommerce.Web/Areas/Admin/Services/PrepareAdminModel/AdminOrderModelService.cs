using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Services.PrepareModelServices.PrepareAdminModel;
using TCommerce.Web.Areas.Admin.Models.Orders;

namespace TCommerce.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminOrderModelService
    {
        Task<OrderSearchModel> PrepareOrderSearchModelAsync(OrderSearchModel searchModel);
    }
    public class AdminOrderModelService : IAdminOrderModelService
    {
        private readonly IBaseAdminModelService _baseAdminModelService;
        private readonly IPaymentService _paymentService;

        public AdminOrderModelService(IBaseAdminModelService baseAdminModelService, IPaymentService paymentService)
        {
            _baseAdminModelService = baseAdminModelService;
            _paymentService = paymentService;
        }

        public async Task<OrderSearchModel> PrepareOrderSearchModelAsync(OrderSearchModel searchModel)
        {
            ArgumentNullException.ThrowIfNull(searchModel);

            await _baseAdminModelService.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
            if (searchModel.AvailableOrderStatuses.Any())
            {
                if (searchModel.OrderStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.OrderStatusIds.Select(id => id.ToString());
                    var statusItems = searchModel.AvailableOrderStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                    foreach (var statusItem in statusItems)
                    {
                        statusItem.Selected = true;
                    }
                }
                else
                    searchModel.AvailableOrderStatuses.FirstOrDefault().Selected = true;
            }

            await _baseAdminModelService.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
            if (searchModel.AvailablePaymentStatuses.Any())
            {
                if (searchModel.PaymentStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.PaymentStatusIds.Select(id => id.ToString());
                    var statusItems = searchModel.AvailablePaymentStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                    foreach (var statusItem in statusItems)
                    {
                        statusItem.Selected = true;
                    }
                }
                else
                    searchModel.AvailablePaymentStatuses.FirstOrDefault().Selected = true;
            }

            //prepare available payment methods
            searchModel.AvailablePaymentMethods = (_paymentService.GetAllPaymentMethods()).Select(x => new SelectListItem()
            {
                Value = x.PaymentMethodSystemName,
                Text = x.Name,
                Selected = x.Selected,
            }).ToList();

            return searchModel;
        }
    }
}
