﻿using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Models
{
    public class AccountNavigationModel : BaseEntity
    {
        public AccountNavigationModel()
        {
            AccountNavigationItems = new List<AccountNavigationItemModel>();
        }

        public List<AccountNavigationItemModel> AccountNavigationItems { get; set; }

        public int SelectedTab { get; set; }
    }

    public class AccountNavigationItemModel : BaseEntity
    {
        public string RouteName { get; set; }
        public string Title { get; set; }
        public int Tab { get; set; }
        public string ItemClass { get; set; }
    }

    public enum AccountNavigationEnum
    {
        Info = 1,
        Addresses = 2,
        ChangePassword = 3,
        Orders = 4
    }
}
