using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model.Common;
using T.Web.Models;
using T.Web.Services.AddressServices;

namespace T.Web.Services.PrepareModelServices
{
    public interface IAccountModelService
    {
        AccountNavigationModel PrepareAccountNavigationModel(int selectedTabId = 0);
        Task<DeliveryAddressModel> PrepareDeliveryAddressModel(DeliveryAddress address, DeliveryAddressModel model);
    }
    public class AccountModelService : IAccountModelService
    {
        private readonly IAddressService _addressService;
        private readonly IBaseModelService _baseModelService;

        public AccountModelService(IAddressService addressService, IBaseModelService baseModelService)
        {
            _addressService = addressService;
            _baseModelService = baseModelService;
        }

        public AccountNavigationModel PrepareAccountNavigationModel(int selectedTabId = 0)
        {
            var model = new AccountNavigationModel();

            model.AccountNavigationItems.Add(new AccountNavigationItemModel
            {
                RouteName = "AccountInfo",
                Title = "Account Info",
                Tab = (int)AccountNavigationEnum.Info,
                ItemClass = "account-info"
            });

            model.AccountNavigationItems.Add(new AccountNavigationItemModel
            {
                RouteName = "AccountAddresses",
                Title = "Account Addresses",
                Tab = (int)AccountNavigationEnum.Addresses,
                ItemClass = "account-addresses"
            });

            model.SelectedTab = selectedTabId;

            return model;
        }

        public async Task<DeliveryAddressModel> PrepareDeliveryAddressModel(DeliveryAddress address, DeliveryAddressModel model)
        {
            if (address is not null)
            {
                model ??= new DeliveryAddressModel()
                {
                    Id = address.Id
                };

                model.FirstName = address.FirstName;
                model.LastName = address.LastName;
                model.PhoneNumber = address.PhoneNumber;
                model.AddressDetails = address.AddressDetails;
                model.DeliveryAddressType = address.DeliveryAddressType;
                model.ProvinceId = address.ProvinceId;
                model.DistrictId = address.DistrictId;
                model.CommuneId = address.CommuneId;
                model.DeliveryAddressTypeId = address.DeliveryAddressTypeId;
                model.DeliveryAddressType = address.DeliveryAddressType;
                model.IsDefault = address.IsDefault;
            }

            await _baseModelService.PrepareSelectListProvinceAsync(model.AvaiableProvinces, true, "Chọn Tỉnh/Thành phố");

            await _baseModelService.PrepareSelectListDistrictAsync(model.AvaiableDistricts, address is not null ? address.ProvinceId : 0, true, "Chọn Quận/Huyện");

            await _baseModelService.PrepareSelectListCommuneAsync(model.AvaiableCommunes, address is not null ? address.DistrictId : 0, true, "Chọn Phường/Xã");

            return model;
        }
    }
}
