using AspNetCoreRateLimit;
using TCommerce.Services.DbManageServices;

namespace TCommerce.Web.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void ConfigureCustomMiddleware(this IApplicationBuilder app)
        {
            if (!DatabaseManager.IsDatabaseInstalled())
            {
                app.UseMiddleware<DatabaseCheckMiddleware>();
                return;
            }

            app.UseIpRateLimiting();
            //app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
