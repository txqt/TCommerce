using Microsoft.AspNetCore.Mvc;
using TCommerce.Web.Areas.Admin.Services.PrepareAdminModel;

namespace TCommerce.Web.Areas.Admin.Components
{
    public class CommonStatisticsViewComponent : ViewComponent
    {
        private readonly IAdminCommonModelService _adminCommonModelService;

        public CommonStatisticsViewComponent(IAdminCommonModelService adminCommonModelService)
        {
            _adminCommonModelService = adminCommonModelService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _adminCommonModelService.PrepareCommonStatisticsModelAsync());
        }
    }
}
