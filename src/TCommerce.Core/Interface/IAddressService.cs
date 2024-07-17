using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Interface
{
    public interface IAddressService
    {
        Task<List<VietNamProvince>> GetAllProvinces();
        Task<Address?> GetAddressByIdAsync(int id);
        Task<List<VietNamDistrict>> GetDistrictsByProvinceId(int provinceId);
        Task<List<VietNamCommune>> GetCommunesByDistrictId(int districtId);
        Task CreateAddressAsync(Address deliveryAddress);
        Task UpdateAddressAsync(Address deliveryAddress);
        Task BulkCreateProvince(IEnumerable<VietNamProvince> provinceList);
        Task BulkCreateDistrict(IEnumerable<VietNamDistrict> districtList);
        Task BulkCreateCommune(IEnumerable<VietNamCommune> communeList);
        Task<VietNamProvince?> GetProvinceByIdAsync(int provinceId);
        Task<VietNamDistrict?> GetDistricteByIdAsync(int districtId);
        Task<VietNamCommune?> GetCommuneByIdAsync(int communeId);
        Task DeleteAddressAsync(int id);
    }
}
