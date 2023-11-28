using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Security;

namespace T.Web.Component
{
    public class HomePageProductsViewComponent : ViewComponent
    {
        public HomePageProductsViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
