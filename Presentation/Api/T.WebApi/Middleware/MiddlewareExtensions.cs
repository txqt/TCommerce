using T.WebApi.Middleware.ErrorHandlings;

namespace T.WebApi.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
