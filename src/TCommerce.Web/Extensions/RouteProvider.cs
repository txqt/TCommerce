using Microsoft.AspNetCore.Routing;
using TCommerce.Services.DbManageServices;
using TCommerce.Web.Routing;

namespace TCommerce.Web.Extensions
{
    public static class RouteProvider
    {
        public static IApplicationBuilder RegisterRoutes(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                if (DatabaseManager.IsDatabaseInstalled())
                {
                    endpoints.MapDynamicControllerRoute<SlugRouteTransformer>("{slug}");
                }

                endpoints.MapControllerRoute(name: "cart",
                        pattern: "cart",
                        defaults: new { controller = "ShoppingCart", action = "Cart" });

                endpoints.MapControllerRoute(name: "cart",
                        pattern: "cart/clear",
                        defaults: new { controller = "ShoppingCart", action = "ClearShoppingCart" });

                endpoints.MapControllerRoute(name: "PageNotFound",
                    pattern: $"page-not-found",
                    defaults: new { controller = "Common", action = "PageNotFound" });

                endpoints.MapControllerRoute(name: "ProductSearch",
                    pattern: $"search/",
                    defaults: new { controller = "Catalog", action = "Search" });

                endpoints.MapControllerRoute(name: "GetCategoryProducts",
                    pattern: $"category/products/",
                    defaults: new { controller = "Catalog", action = "GetCategoryProducts" });

                endpoints.MapControllerRoute(name: "GetManufacturerProducts",
                    pattern: $"manufacturer/products/",
                    defaults: new { controller = "Catalog", action = "GetManufacturerProducts" });

                endpoints.MapControllerRoute(name: "SearchProducts",
                    pattern: "product/search",
                    defaults: new { controller = "Catalog", action = "SearchProducts" });

                endpoints.MapControllerRoute(name: "AccountInfo",
                    pattern: $"account/info",
                    defaults: new { controller = "Account", action = "Info" });

                endpoints.MapControllerRoute(name: "AccountAddresses",
                    pattern: $"account/addresses",
                    defaults: new { controller = "Account", action = "Addresses" });

                endpoints.MapControllerRoute(name: "AccountOrders",
                    pattern: $"order/history",
                    defaults: new { controller = "Account", action = "Orders" });

                endpoints.MapControllerRoute(name: "CreateAddress",
                    pattern: $"account/address/create",
                    defaults: new { controller = "Account", action = "CreateAddress" });

                endpoints.MapControllerRoute(name: "Login",
                    pattern: $"login/",
                    defaults: new { controller = "Account", action = "Login" });

                endpoints.MapControllerRoute(name: "Register",
                    pattern: $"register/",
                    defaults: new { controller = "Account", action = "Register" });

                endpoints.MapControllerRoute(name: "Logout",
                    pattern: $"logout",
                    defaults: new { controller = "Account", action = "Logout" });

                endpoints.MapControllerRoute(name: "HomeAdmin",
                    pattern: $"admin",
                    defaults: new { areas = "admin", controller = "home", action = "index" });

                endpoints.MapControllerRoute(name: "CheckoutConfirm",
                    pattern: $"checkout/confirm",
                    defaults: new { controller = "Checkout", action = "Confirm" });
            });

            return app;
        }
    }
}
