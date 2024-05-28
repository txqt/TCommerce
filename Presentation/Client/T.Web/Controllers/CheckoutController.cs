using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Common;
using T.Web.Models;
using T.Web.Services.AddressServices;
using T.Web.Services.PrepareModelServices;
using T.Web.Services.UserService;

namespace T.Web.Controllers
{
    public class CheckoutController : BaseController
    {
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IShoppingCartModelService _shoppingCartModelService;
        private readonly IAccountModelService _accountModelService;
        private readonly IMapper _mapper;

        public CheckoutController(IAddressService addressService, IUserService userService, IShoppingCartModelService shoppingCartModelService, IMapper mapper, IAccountModelService accountModelService)
        {
            _addressService = addressService;
            _userService = userService;
            _shoppingCartModelService = shoppingCartModelService;
            _mapper = mapper;
            _accountModelService = accountModelService;
        }

        public async Task<IActionResult> Payment()
        {
            var model = new CheckoutPaymentModel();

            var addresses = await _userService.GetOwnAddressesAsync();

            if (addresses != null && addresses.Any())
            {
                var shippingAddress = new CheckoutShippingAddressModel
                {
                    DefaultShippingAddress = addresses.FirstOrDefault(x => x.IsDefault),
                    ExistingAddresses = addresses.Where(x => !x.IsDefault).ToList()
                };

                model.ShippingAddress = shippingAddress;
            }
            else
            {
                model.ShippingAddress = null;
            }

            model.Cart = await _shoppingCartModelService.PrepareShoppingCartModelAsync();
            model.ShippingAddress.NewShippingAddress = await _accountModelService.PrepareDeliveryAddressModel(null, new DeliveryAddressModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SelectAddress(int id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);

            var addresses = await _userService.GetOwnAddressesAsync();

            if(addresses.Where(x=>x.Id != address.Id) is null)
            {
                return Json(new { success = false });
            }

            address.IsDefault = true;

            var result = await _userService.UpdateUserAddressAsync(address);

            SetStatusMessage(result.Success ? "Success" : "Failed");

            ViewBag.RefreshPage = true;

            return Json(new { success = result.Success });
        }

        [HttpPost]
        public async Task<IActionResult> NewDefaultAddress(DeliveryAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _accountModelService.PrepareDeliveryAddressModel(null, model);
                return View(model);
            }

            var address = _mapper.Map<DeliveryAddress>(model);

            address.CreatedOnUtc = DateTime.Now;
            address.IsDefault = true;

            var result = await _userService.CreateUserAddressAsync(address);

            SetStatusMessage(result.Success ? "Success" : "Failed");

            return RedirectToAction(nameof(Payment));
        }
    }
}
