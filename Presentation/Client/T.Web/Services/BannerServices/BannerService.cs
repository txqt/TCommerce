using T.Library.Model.Banners;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.Web.Helpers;

namespace T.Web.Services.BannerServices
{
    public class BannerService : HttpClientHelper, IBannerService
    {
        private readonly string defaultApiRoute = "api/banner/";
        public BannerService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<Banner>> GetAllBannerAsync()
        {
            return await GetAsync<List<Banner>>($"{defaultApiRoute}");
        }

        public async Task<Banner> GetBannerByIdAsync(int id)
        {
            return await GetAsync<Banner>($"{defaultApiRoute + id}");
        }

        public async Task<ServiceResponse<bool>> CreateBannerAsync(BannerViewModel banner)
        {
            return await PostWithFormFileAsync<ServiceResponse<bool>>($"{defaultApiRoute}", banner, banner.ImageFile);
        }

        public async Task<ServiceResponse<bool>> UpdateBannerAsync(BannerViewModel banner)
        {
            return await PutWithFormFileAsync<ServiceResponse<bool>>($"{defaultApiRoute}", banner, banner.ImageFile);
        }

        public async Task<ServiceResponse<bool>> DeleteBannerByIdAsync(int id)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"{defaultApiRoute + id}");
        }
    }
}
