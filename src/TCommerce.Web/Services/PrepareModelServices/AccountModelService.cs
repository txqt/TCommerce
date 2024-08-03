using AutoMapper;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Web.Models;

namespace TCommerce.Web.PrepareModelServices
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
        private readonly IMapper _mapper;

        public AccountModelService(IAddressService addressService, IBaseModelService baseModelService, IMapper mapper)
        {
            _addressService = addressService;
            _baseModelService = baseModelService;
            _mapper = mapper;
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

            model.AccountNavigationItems.Add(new AccountNavigationItemModel
            {
                RouteName = "AccountChangePassword",
                Title = "Account Change Password",
                Tab = (int)AccountNavigationEnum.ChangePassword,
                ItemClass = "account-change-password"
            });

            model.SelectedTab = selectedTabId;

            return model;
        }

        public async Task<AddressModel> PrepareAddressModel(Address address, AddressModel model)
        {
            if (address is not null)
            {
                model ??= new AddressModel()
                {
                    Id = address.Id
                };

                _mapper.Map(address, model);
            }

            await _baseModelService.PrepareSelectListProvinceAsync(model.AvaiableProvinces, true, "Chọn Tỉnh/Thành phố");

            await _baseModelService.PrepareSelectListDistrictAsync(model.AvaiableDistricts, address is not null ? address.ProvinceId : 0, true, "Chọn Quận/Huyện");

            await _baseModelService.PrepareSelectListCommuneAsync(model.AvaiableCommunes, address is not null ? address.DistrictId : 0, true, "Chọn Phường/Xã");

            return model;
        }
    }
}
