﻿using Microsoft.AspNetCore.Mvc;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Component
{
    public class AccountNavigationViewComponent : ViewComponent
    {
        private readonly IAccountModelService _accountModelService;

        public AccountNavigationViewComponent(IAccountModelService accountModelService)
        {
            _accountModelService = accountModelService;
        }

        public IViewComponentResult Invoke(int selectedTabId = 0)
        {
            var model = _accountModelService.PrepareAccountNavigationModel(selectedTabId);
            return View(model);
        }
    }
}
