using Microsoft.AspNetCore.Mvc;

namespace T.Web.Component
{
    public class HeaderTopViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
