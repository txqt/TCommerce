using AutoMapper;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Banners;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Web.Areas.Admin.Models.Banners;

namespace TCommerce.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminBannerModelService
    {
        Task<BannerViewModel> PrepareBannerModel(BannerViewModel model, Banner banner);
    }
    public class AdminBannerModelService : IAdminBannerModelService
    {
        private readonly IBannerService _bannerService;
        private readonly IMapper _mapper;
        private readonly IPictureService _pictureService;

        public AdminBannerModelService(IMapper mapper, IBannerService bannerService, IPictureService pictureService)
        {
            _mapper = mapper;
            _bannerService = bannerService;
            _pictureService = pictureService;
        }

        public async Task<BannerViewModel> PrepareBannerModel(BannerViewModel model, Banner banner)
        {
            if (banner is not null)
            {
                model ??= new BannerViewModel()
                {
                    Id = banner.Id
                };
                _mapper.Map(banner, model);

                if(banner.PictureId > 0)
                {
                    model.PictureUrl = (await _pictureService.GetPictureByIdAsync(banner.PictureId)).UrlPath;
                }
            }

            return model;
        }
    }
}
