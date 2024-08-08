using Microsoft.AspNetCore.Identity;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.Roles;
using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Users;
using TCommerce.Web.Areas.Admin.Models.Common;

namespace TCommerce.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminCommonModelService
    {
        Task<CommonStatisticsModel> PrepareCommonStatisticsModelAsync();
    }
    public class AdminCommonModelService : IAdminCommonModelService
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public AdminCommonModelService(IOrderService orderService, IUserService userService, IProductService productService)
        {
            _orderService = orderService;
            _userService = userService;
            _productService = productService;
        }

        public async Task<CommonStatisticsModel> PrepareCommonStatisticsModelAsync()
        {
            var model = new CommonStatisticsModel
            {
                NumberOfOrders = (await _orderService.SearchOrdersAsync(new OrderParameters()
                {
                })).Count
            };
            
            model.NumberOfCustomers = (await _userService.GetAllUsersAsync(new UserParameters()
            {
                Roles = new List<string>
                {
                    RoleName.Registerd
                }
            })).Count;

            model.NumberOfLowStockProducts =
                (await _productService.GetLowStockProductsAsync()).Count;

            return model;
        }
    }
}
