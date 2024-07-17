namespace TCommerce.Web.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void ConfigureCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<DatabaseCheckMiddleware>();
        }
    }
}
