using T.Web.CusomMiddleware;

namespace T.WebApi.Middleware.ErrorHandlings
{
    public static class CustomMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtMiddleware>();
        }
        public static void ConfigureJwtMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtMiddleware>();
        }
    }
}
