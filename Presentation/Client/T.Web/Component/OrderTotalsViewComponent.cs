using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Web.Services.PrepareModelServices;
using T.Web.Services.ShoppingCartServices;
using T.Web.Services.UserService;

namespace T.Web.Component
{
    public class OrderTotalsViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartModelService _shoppingCartModel;
        private readonly IUserService _userService;

        public OrderTotalsViewComponent(IShoppingCartService shoppingCartService, IShoppingCartModelService shoppingCartModel, IUserService userService)
        {
            _shoppingCartService = shoppingCartService;
            _shoppingCartModel = shoppingCartModel;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _userService.GetCurrentUser(), ShoppingCartType.ShoppingCart);

            var model = await _shoppingCartModel.PrepareOrderTotalsModelAsync(cart);

            return View(model);
        }
    }
}
