using T.Library.Model.Banners;
using T.Library.Model.Interface;
using T.Library.Model.Response;

namespace T.Web.Services.BannerServices
{
    public class BannerService : IBannerService
    {
        public Task<List<Banner>> GetAllBannerAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<Banner>> GetBannerByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> CreateBannerAsync(Banner banner)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> UpdateBannerAsync(Banner banner)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> DeleteBannerByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
