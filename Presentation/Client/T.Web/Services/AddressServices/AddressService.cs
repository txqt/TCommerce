using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Web.Helpers;

namespace T.Web.Services.AddressServices
{
    public interface IAddressService : IAddressServiceCommon
    {

    }
    public class AddressService : HttpClientHelper, IAddressService
    {
        public AddressService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<VietNamProvince>> GetAllProvinces()
        {
            return await GetAsync<List<VietNamProvince>>("api/address/province");
        }

        public async Task<List<VietNamDistrict>> GetDistrictsByProvinceId(int provinceId)
        {
            return await GetAsync<List<VietNamDistrict>>($"api/address/province/district/{provinceId}");
        }

        public async Task<List<VietNamCommune>> GetCommunesByDistrictId(int districtId)
        {
            return await GetAsync<List<VietNamCommune>>($"api/address/province/commune/{districtId}");
        }

        public async Task<Address> GetAddressByIdAsync(int id)
        {
            return await GetAsync<Address>($"api/address/{id}");
        }
    }
}
