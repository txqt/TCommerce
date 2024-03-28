using Microsoft.AspNetCore.Mvc;

namespace T.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string RouteName(this IUrlHelper urlHelper, string routeName)
        {
            return urlHelper.RouteUrl(routeName, values: null, protocol: null, host: null, fragment: null);
        }
    }
}
