using Microsoft.AspNetCore.Mvc;
using T.Web.Services.PrepareModelServices;

namespace T.Web.Component
{
    public class AccountNavigationViewComponent : ViewComponent
    {
        private readonly IAccountModelService _accountModelService;

        public AccountNavigationViewComponent(IAccountModelService accountModelService)
        {
            _accountModelService = accountModelService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int selectedTabId = 0)
        {
            var model = await _accountModelService.PrepareCustomerNavigationModelAsync(selectedTabId);
            return View(model);
        }
    }
}
