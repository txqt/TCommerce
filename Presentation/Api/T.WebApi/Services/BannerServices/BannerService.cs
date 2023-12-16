using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using T.Library.Model;
using T.Library.Model.Banners;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.WebApi.Services.IRepositoryServices;
using T.WebApi.Services.PictureServices;

namespace T.WebApi.Services.BannerServices
{
    public class BannerService : IBannerService
    {
        private readonly IRepository<Banner> _bannerRepository;
        private readonly IPictureService _pictureService;
        private readonly IMapper _mapper;
        private string? APIUrl;
        private readonly IConfiguration _configuration;
        public BannerService(IRepository<Banner> bannerRepository, IMapper mapper, IPictureService pictureService, IConfiguration configuration)
        {
            _bannerRepository = bannerRepository;
            _mapper = mapper;
            _pictureService = pictureService;
            _configuration = configuration;
            APIUrl = _configuration.GetSection("Url:APIUrl").Value;
        }

        public async Task<List<Banner>> GetAllBannerAsync()
        {
            var result = await _bannerRepository.Table.Include(x => x.Picture).ToListAsync();
            foreach (var item in result)
            {
                var temp = item.Picture.UrlPath;
                item.Picture.UrlPath = APIUrl + temp;
            }
            return result;
        }
        public async Task<Banner> GetBannerByIdAsync(int id)
        {
            return await _bannerRepository.GetByIdAsync(id);
        }

        public async Task<ServiceResponse<bool>> CreateBannerAsync(BannerViewModel model)
        {
            var pictureResult = await _pictureService.SavePictureWithEncryptFileName(model.ImageFile);
            if (!pictureResult.Success)
            {
                return ErrorResponse(pictureResult.Message);
            }

            try
            {
                var banner = _mapper.Map<Banner>(model);
                banner.PictureId = pictureResult.Data;
                await _bannerRepository.CreateAsync(banner);
                return SuccessResponse();
            }
            catch (Exception ex)
            {
                await _pictureService.DeletePictureByIdAsync(pictureResult.Data);
                return ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateBannerAsync(BannerViewModel model)
        {
            var banner = await _bannerRepository.GetByIdAsync(model.Id);
            if (banner == null)
            {
                return ErrorResponse("Banner not found");
            }

            int? oldPictureId = null;
            if (model.ImageFile != null)
            {
                var pictureResult = await _pictureService.SavePictureWithEncryptFileName(model.ImageFile);
                if (!pictureResult.Success)
                {
                    return ErrorResponse(pictureResult.Message);
                }

                // Store the old picture id and update the PictureId
                oldPictureId = banner.PictureId;
                banner.PictureId = pictureResult.Data;
            }

            try
            {
                // Update the banner
                banner = _mapper.Map(model, banner);
                await _bannerRepository.UpdateAsync(banner);

                // Delete the old picture
                if (oldPictureId.HasValue)
                {
                    await _pictureService.DeletePictureByIdAsync(oldPictureId.Value);
                }

                return SuccessResponse();
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteBannerByIdAsync(int id)
        {
            try
            {
                var banner = await _bannerRepository.GetByIdAsync(id);
                if(banner.PictureId > 0)
                {
                    var deletePictureResult = await _pictureService.DeletePictureByIdAsync(banner.PictureId);
                    if (!deletePictureResult.Success)
                    {
                        return ErrorResponse("Cannot delete picture");
                    }
                }

                await _bannerRepository.DeleteAsync(id);
                return SuccessResponse();
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        private ServiceResponse<bool> ErrorResponse(string message)
        {
            return new ServiceErrorResponse<bool>() { Message = message, Success = false };
        }
        private ServiceResponse<bool> SuccessResponse()
        {
            return new ServiceSuccessResponse<bool>() { Success = true };
        }
    }
}
