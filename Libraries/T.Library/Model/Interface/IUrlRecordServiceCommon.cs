using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;
using T.Library.Model.Response;
using T.Library.Model.Seo;
using T.Library.Model.ViewsModel;

namespace T.Library.Model.Interface
{
    public interface IUrlRecordServiceCommon
    {
        Task<List<UrlRecord>> GetAllAsync();
        Task<UrlRecord> GetByIdAsync(int id);
        Task<ServiceResponse<bool>> CreateUrlRecordAsync(UrlRecord model);
        Task<ServiceResponse<bool>> UpdateUrlRecordAsync(UrlRecord model);
        Task<ServiceResponse<bool>> DeleteUrlRecordByIdAsync(int id);
        Task<UrlRecord> GetBySlugAsync(string slug);
        Task<string> GetActiveSlugAsync(int entityId, string entityName);
        
    }
}
