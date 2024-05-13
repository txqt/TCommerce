using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Banners;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;

namespace T.Library.Model.Interface
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
