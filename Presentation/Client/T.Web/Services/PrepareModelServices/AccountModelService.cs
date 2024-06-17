using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model.Common;
using T.Web.Models;
using T.Web.Services.AddressServices;

namespace T.Web.Services.PrepareModelServices
{
    public interface IAccountModelService
    {
        AccountNavigationModel PrepareAccountNavigationModel(int selectedTabId = 0);
        Task<AddressModel> PrepareAddressModel(Address address, AddressModel model);
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

        public async Task<AddressModel> PrepareAddressModel(Address address, AddressModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            if (address is not null)
            {
                model ??= new AddressModel()
                {
                    Id = address.Id
                };

                model.FirstName = address.FirstName;
                model.LastName = address.LastName;
                model.PhoneNumber = address.PhoneNumber;
                model.AddressDetails = address.AddressDetails;
                model.AddressType = address.AddressType;
                model.ProvinceId = address.ProvinceId;
                model.DistrictId = address.DistrictId;
                model.CommuneId = address.CommuneId;
                model.AddressTypeId = address.AddressTypeId;
                model.AddressType = address.AddressType;
                model.IsDefault = address.IsDefault;
            }

            await _baseModelService.PrepareSelectListProvinceAsync(model.AvaiableProvinces, true, "Chọn Tỉnh/Thành phố");

            await _baseModelService.PrepareSelectListDistrictAsync(model.AvaiableDistricts, address is not null ? address.ProvinceId : 0, true, "Chọn Quận/Huyện");

            await _baseModelService.PrepareSelectListCommuneAsync(model.AvaiableCommunes, address is not null ? address.DistrictId : 0, true, "Chọn Phường/Xã");

            return model;
        }
    }
}
