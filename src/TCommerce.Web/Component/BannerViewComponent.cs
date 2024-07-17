using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;

namespace TCommerce.Web.Component
{
    public class BannerViewComponent : ViewComponent
    {
        private readonly IBannerService _bannerService;

        public BannerViewComponent(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var banners = await _bannerService.GetAllBannerAsync();
            return View(banners);
        }
    }
}
