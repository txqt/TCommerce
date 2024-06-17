using T.Web.Routing;

namespace T.Web.Extensions
{
    public static class RouteProvider
    {
        public static IApplicationBuilder RegisterRoutes(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDynamicControllerRoute<SlugRouteTransformer>("{slug}");

                endpoints.MapControllerRoute(name: "cart",
                        pattern: "cart",
                        defaults: new { controller = "ShoppingCart", action = "Cart" });

                endpoints.MapControllerRoute(name: "PageNotFound",
                    pattern: $"page-not-found",
                    defaults: new { controller = "Common", action = "PageNotFound" });

                endpoints.MapControllerRoute(name: "GetCategoryProducts",
                    pattern: $"category/products/",
                    defaults: new { controller = "Catalog", action = "GetCategoryProducts" });

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

                endpoints.MapControllerRoute(name: "SignInOrSignUp",
                    pattern: $"sign-in-sign-up",
                    defaults: new { controller = "Account", action = "SignInOrSignUp" });

                endpoints.MapControllerRoute(name: "Logout",
                    pattern: $"logout",
                    defaults: new { controller = "Account", action = "Logout" });

                endpoints.MapControllerRoute(name: "HomeAdmin",
                    pattern: $"admin",
                    defaults: new { areas = "admin", controller = "home", action = "index"  });
            });

            return app;
        }
    }
}
