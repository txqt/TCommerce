using TCommerce.Services.DbManageServices;

namespace TCommerce.Web.Middleware
{
    public class DatabaseCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public DatabaseCheckMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path != "/Install" && !context.Request.Path.StartsWithSegments("/Install"))
            {
                if (!DatabaseManager.IsDatabaseInstalled())
                {
                    context.Response.Redirect("/Install");
                    return;
                }
            }

            await _next(context);
        }
    }
}
