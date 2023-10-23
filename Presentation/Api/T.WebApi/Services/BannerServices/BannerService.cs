using System.Reflection;
using T.Library.Model.Banners;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.BannerServices
{
    public class BannerService : IBannerService
    {
        private readonly IRepository<Banner> _bannerRepository;
        public BannerService(IRepository<Banner> bannerRepository)
        {
            _bannerRepository = bannerRepository;
        }

        public async Task<List<Banner>> GetAllBannerAsync()
        {
            return (await _bannerRepository.GetAllAsync()).ToList();
        }
        public async Task<ServiceResponse<Banner>> GetBannerByIdAsync(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);

            return new ServiceResponse<Banner>()
            {
                Data = banner,
                Success = true,
            };
        }

        public async Task<ServiceResponse<bool>> CreateBannerAsync(Banner banner)
        {
            try
            {
                await _bannerRepository.CreateAsync(banner);
                return SuccessResponse();
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateBannerAsync(Banner banner)
        {
            try
            {
                await _bannerRepository.UpdateAsync(banner);
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
            return new ServiceErrorResponse<bool>() { Message = message };
        }
        private ServiceResponse<bool> SuccessResponse()
        {
            return new ServiceSuccessResponse<bool>();
        }

    }
}
