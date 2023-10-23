using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Banners;
using T.Library.Model.Response;

namespace T.Library.Model.Interface
{
    public interface IBannerService
    {
        Task<List<Banner>> GetAllBannerAsync();
        Task<ServiceResponse<Banner>> GetBannerByIdAsync(int id);
        Task<ServiceResponse<bool>> CreateBannerAsync(Banner banner);
        Task<ServiceResponse<bool>> UpdateBannerAsync(Banner banner);
        Task<ServiceResponse<bool>> DeleteBannerByIdAsync(int id);
    }
}
