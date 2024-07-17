using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Banners;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Services.IRepositoryServices;

namespace TCommerce.Services.BannerServices
{
    public class BannerService : IBannerService
    {
        private readonly IRepository<Banner> _bannerRepository;
        private readonly IPictureService _pictureService;
        private readonly IMapper _mapper;
        private string? ClientUrl;
        private readonly IConfiguration _configuration;
        public BannerService(IRepository<Banner> bannerRepository, IMapper mapper, IPictureService pictureService, IConfiguration configuration)
        {
            _bannerRepository = bannerRepository;
            _mapper = mapper;
            _pictureService = pictureService;
            _configuration = configuration;
            ClientUrl = _configuration.GetSection("Url:ClientUrl").Value;
        }

        public async Task<List<Banner>> GetAllBannerAsync()
        {
            var result = await _bannerRepository.Table.ToListAsync();
            return result;
        }
        public async Task<Banner?> GetBannerByIdAsync(int id)
        {
            return await _bannerRepository.GetByIdAsync(id);
        }

        public async Task<ServiceResponse<bool>> CreateBannerAsync(BannerViewModel model)
        {
            var pictureId = 0;

            if(model.ImageFile is not null)
            {
                var pictureResult = await _pictureService.SavePictureWithEncryptFileName(model.ImageFile);

                pictureId = pictureResult.Data;

                if (!pictureResult.Success)
                {
                    return ErrorResponse(pictureResult.Message);
                }
            }

            try
            {
                var banner = _mapper.Map<Banner>(model);

                banner.PictureId = pictureId;

                await _bannerRepository.CreateAsync(banner);

                return SuccessResponse();
            }
            catch (Exception ex)
            {
                await _pictureService.DeletePictureByIdAsync(pictureId);
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

                if (banner is not null)
                {
                    await _bannerRepository.DeleteAsync(id);

                    var pictureId = banner.PictureId;

                    if (banner.PictureId > 0)
                    {
                        var deletePictureResult = await _pictureService.DeletePictureByIdAsync(pictureId);
                        if (!deletePictureResult.Success)
                        {
                            return ErrorResponse("Cannot delete picture");
                        }
                    }
                }

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
