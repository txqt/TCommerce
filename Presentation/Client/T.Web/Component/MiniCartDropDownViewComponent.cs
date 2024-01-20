using Microsoft.AspNetCore.Mvc;
using T.Web.Services.PrepareModelServices;

namespace T.Web.Component
{
    public class MiniCartDropDownViewComponent : ViewComponent
    {
        private readonly IShoppingCartModelService _sciModelService;
        public MiniCartDropDownViewComponent(IShoppingCartModelService sciModelService)
        {
            _sciModelService = sciModelService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _sciModelService.PrepareMiniShoppingCartModelAsync();
            return View(model);
        }
    }
}
