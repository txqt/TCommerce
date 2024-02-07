using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using T.Library.Model.Interface;
using T.Library.Model.JwtToken;
using T.Web.Common;
using T.Web.Helpers;
using T.Web.Routing;
using T.Web.Services;
using T.Web.Services.BannerServices;
using T.Web.Services.CategoryService;
using T.Web.Services.Database;
using T.Web.Services.ManufacturerServices;
using T.Web.Services.PictureServices;
using T.Web.Services.PrepareModel;
using T.Web.Services.PrepareModelServices;
using T.Web.Services.PrepareModelServices.PrepareAdminModel;
using T.Web.Services.ProductService;
using T.Web.Services.SecurityServices;
using T.Web.Services.ShoppingCartServices;
using T.Web.Services.UrlRecordService;
using T.Web.Services.UserRegistrationServices;
using T.Web.Services.UserService;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation().AddJsonOptions(options =>
        {
            //options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        }); ;
        builder.Services.AddTransient<JwtHandler>();
        builder.Services.AddHttpClient("", sp =>
        {
            sp.BaseAddress = new Uri(builder.Configuration.GetSection("Url:ApiUrl").Value);
        })
            .AddHttpMessageHandler<JwtHandler>()
            .AddHttpMessageHandler<UnauthorizedResponseHandler>();

        builder.Services.AddTransient<IDatabaseControl, DatabaseControl>();
        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        builder.Services.AddTransient<IUserRegistrationService, UserRegistrationService>();
        builder.Services.AddTransient<IProductService, ProductService>();
        builder.Services.AddTransient<IProductAttributeCommon, ProductAttributeService>();
        builder.Services.AddTransient<ISecurityService, SecurityService>();
        builder.Services.AddTransient<IProductModelService, ProductModelService>();
        builder.Services.AddTransient<ICategoryModelService, CategoryModelService>();
        builder.Services.AddTransient<IBannerModelService, BannerModelService>();
        builder.Services.AddTransient<IUserModelService, UserModelService>();
        builder.Services.AddTransient<ICategoryServiceCommon, CategoryService>();
        builder.Services.AddTransient<IProductCategoryService, ProductCategoryService>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IBannerService, BannerService>();
        builder.Services.AddTransient<IUrlRecordService, UrlRecordService>();
        builder.Services.AddTransient<IPictureService, PictureService>();
        builder.Services.AddSingleton<HttpClientHelper>();
        builder.Services.AddTransient<UnauthorizedResponseHandler>();
        builder.Services.AddTransient<SlugRouteTransformer>();
        builder.Services.AddTransient<IShoppingCartModelService, ShoppingCartModelService>();
        builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
        builder.Services.AddTransient<IManufacturerService, ManufacturerService>();
        builder.Services.AddTransient<IManufacturerModelService, ManufacturerModelService>();
        builder.Services.AddTransient<IBaseAdminModelService, BaseAdminModelService>();
        builder.Services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });


        var jwtSection = builder.Configuration.GetSection("Authorization");
        var jwtOptions = new AuthorizationOptionsConfig();
        jwtSection.Bind(jwtOptions);
        builder.Services.Configure<AuthorizationOptionsConfig>(jwtSection);


        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.LoginPath = "/account/login";
            options.LogoutPath = "/account/logout";
            options.AccessDeniedPath = "/AccessDenied";
        });

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddHttpContextAccessor();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        //app.UseExceptionHandler("/Error");
        //app.UseStatusCodePagesWithReExecute("/Error/{0}");

        app.UseRouting();
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        //app.ConfigureCustomExceptionMiddleware();

        app.MapDynamicControllerRoute<SlugRouteTransformer>("{slug}");

        app.MapControllerRoute(
            name: "MyArea",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(name: "cart",
                pattern: "cart",
                defaults: new { controller = "ShoppingCart", action = "Cart" });

        app.MapControllerRoute(name: "PageNotFound",
            pattern: $"page-not-found",
            defaults: new { controller = "Common", action = "PageNotFound" });

        bool hasRunOnce = false;

        if (app.Environment.IsDevelopment() && !hasRunOnce)
        {
            app.MapGet("/", async context =>
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var _httpClient = services.GetRequiredService<HttpClient>();
                    var result = await _httpClient.GetAsync("api/db-manage/is-installed");

                    if (result.IsSuccessStatusCode)
                    {
                        context.Response.Redirect("/Home/Index");
                        hasRunOnce = true;
                        return;
                    }
                }

                context.Response.Redirect("/Install/Index");
            });
        }
            
        app.Run();
    }
}