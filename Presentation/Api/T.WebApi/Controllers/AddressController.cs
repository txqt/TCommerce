using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Banners;
using T.Library.Model.Common;
using T.WebApi.Services.AddressServices;

namespace T.WebApi.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("province")]
        public async Task<ActionResult<List<VietNamProvince>>> GetAllProvinces()
        {
            return await _addressService.GetAllProvinces();
        }

        [HttpGet("province/district/{provinceId}")]
        public async Task<ActionResult<List<VietNamDistrict>>> GetDistrictsByProvinceId(int provinceId)
        {
            return await _addressService.GetDistrictsByProvinceId(provinceId);
        }

        [HttpGet("province/commune/{districtId}")]
        public async Task<ActionResult<List<VietNamCommune>>> GetCommunesByDistrictId(int districtId)
        {
            return await _addressService.GetCommunesByDistrictId(districtId);
        }
    }
}
