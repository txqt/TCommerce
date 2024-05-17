using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using T.Web.Services.AddressServices;
using T.Web.Services.PrepareModelServices;

namespace T.Web.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IBaseModelService _baseModelService;

        public AddressController(IAddressService addressService, IBaseModelService baseModelService)
        {
            _addressService = addressService;
            _baseModelService = baseModelService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            var districts = new List<SelectListItem>();

            await _baseModelService.PrepareSelectListDistrictAsync(districts, provinceId, true, "Chọn Quận/Huyện");

            return Json(districts);
        }

        public async Task<IActionResult> GetCommunes(int districtId)
        {
            var communes = new List<SelectListItem>();

            await _baseModelService.PrepareSelectListCommuneAsync(communes, districtId, true, "Chọn Phường/Xã");

            return Json(communes);
        }
    }
}
