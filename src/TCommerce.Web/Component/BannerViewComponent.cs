using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Web.Models;

namespace TCommerce.Web.Component
{
    public class BannerViewComponent : ViewComponent
    {
        private readonly IBannerService _bannerService;
        private readonly IMapper _mapper;
        private readonly IPictureService _pictureService;

        public BannerViewComponent(IBannerService bannerService, IMapper mapper, IPictureService pictureService)
        {
            _bannerService = bannerService;
            _mapper = mapper;
            _pictureService = pictureService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var banners = await _bannerService.GetAllBannerAsync();
            var models = _mapper.Map<List<BannerModel>>(banners);
            foreach(var item in models)
            {
                if(item.PictureId > 0)
                {
                    item.Picture = await _pictureService.GetPictureByIdAsync(item.PictureId);
                }
            }
            return View(models);
        }
    }
}
