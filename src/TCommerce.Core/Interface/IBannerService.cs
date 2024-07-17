using TCommerce.Core.Models.Banners;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Core.Interface
{
    public interface IBannerService
    {
        Task<List<Banner>> GetAllBannerAsync();
        Task<Banner?> GetBannerByIdAsync(int id);
        Task<ServiceResponse<bool>> CreateBannerAsync(BannerViewModel model);
        Task<ServiceResponse<bool>> UpdateBannerAsync(BannerViewModel model);
        Task<ServiceResponse<bool>> DeleteBannerByIdAsync(int id);
    }
}
