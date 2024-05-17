using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.Interface
{
    public interface IAddressServiceCommon
    {
        Task<List<VietNamProvince>> GetAllProvinces();
        Task<DeliveryAddress?> GetAddressByIdAsync(int id);
        Task<List<VietNamDistrict>> GetDistrictsByProvinceId(int provinceId);
        Task<List<VietNamCommune>> GetCommunesByDistrictId(int districtId);
    }
}
