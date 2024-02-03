using AutoMapper;
using T.Library.Model.Banners;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;

namespace T.Web.Services.PrepareModelServices.PrepareAdminModel
{
    public interface IBannerModelService
    {
        Task<BannerViewModel> PrepareBannerModelAsync(BannerViewModel model, Banner banner);
    }
    public class BannerModelService : IBannerModelService
    {
        private readonly IBannerService _bannerService;
        private readonly IMapper _mapper;

        public BannerModelService(IMapper mapper, IBannerService bannerService)
        {
            _mapper = mapper;
            _bannerService = bannerService;
        }

        public async Task<BannerViewModel> PrepareBannerModelAsync(BannerViewModel model, Banner banner)
        {
            if (banner is not null)
            {
                model ??= new BannerViewModel()
                {
                    Id = banner.Id
                };
                _mapper.Map(banner, model);
            }

            return model;
        }
    }
}
