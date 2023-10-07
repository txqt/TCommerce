using T.Web.CustomMiddleware.ErrorHandlings;

namespace T.Web.CustomMiddleware
{
    public static class CustomMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
