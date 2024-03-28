using T.Web.Models;

namespace T.Web.Services.PrepareModelServices
{
    public interface IAccountModelService
    {
        Task<AccountNavigationModel> PrepareCustomerNavigationModelAsync(int selectedTabId = 0);
    }
    public class AccountModelService : IAccountModelService
    {
        public async Task<AccountNavigationModel> PrepareCustomerNavigationModelAsync(int selectedTabId = 0)
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
    }
}
