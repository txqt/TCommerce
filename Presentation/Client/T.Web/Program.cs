using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using T.Library.Model.Interface;
using T.Library.Model.JwtToken;
using T.Web.Areas.Admin.Services.PrepareAdminModel;
using T.Web.Areas.Admin.Services.PrepareModel;
using T.Web.Common;
using T.Web.Extensions;
using T.Web.Helpers;
using T.Web.Routing;
using T.Web.Services;
using T.Web.Services.AddressServices;
using T.Web.Services.BannerServices;
using T.Web.Services.CategoryService;
using T.Web.Services.Database;
using T.Web.Services.ManufacturerServices;
using T.Web.Services.PictureServices;
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

        builder.Services.AddControllersWithViews(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(
                                         new SlugifyParameterTransformer()));
        });

        // Add services to the container.
        builder.Services.AddControllersWithViews().AddDataAnnotationsLocalization().AddRazorRuntimeCompilation().AddJsonOptions(options =>
        {
            //options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        builder.Services.AddScoped<JwtHandler>();
        builder.Services.AddHttpClient("", sp =>
        {
            sp.BaseAddress = new Uri(builder.Configuration.GetSection("Url:ApiUrl").Value);
        })
            .AddHttpMessageHandler<JwtHandler>()
            .AddHttpMessageHandler<UnauthorizedResponseHandler>();

        builder.Services.AddScoped<IDatabaseControl, DatabaseControl>();
        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IProductAttributeCommon, ProductAttributeService>();
        builder.Services.AddScoped<ISecurityService, SecurityService>();
        builder.Services.AddScoped<IProductModelService, ProductModelService>();
        builder.Services.AddScoped<ICatalogModelService, CatalogModelService>();
        builder.Services.AddScoped<IAdminCategoryModelService, AdminCategoryModelService>();
        builder.Services.AddScoped<IAdminProductModelService, AdminProductModelService>();
        builder.Services.AddScoped<IAdminBannerModelService, AdminBannerModelService>();
        builder.Services.AddScoped<IAdminUserModelService, AdminUserModelService>();
        builder.Services.AddScoped<ICategoryServiceCommon, CategoryService>();
        builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IBannerService, BannerService>();
        builder.Services.AddScoped<IUrlRecordService, UrlRecordService>();
        builder.Services.AddScoped<IPictureService, PictureService>();
        builder.Services.AddScoped<HttpClientHelper>();
        builder.Services.AddScoped<UnauthorizedResponseHandler>();
        builder.Services.AddScoped<SlugRouteTransformer>();
        builder.Services.AddScoped<IShoppingCartModelService, ShoppingCartModelService>();
        builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
        builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
        builder.Services.AddScoped<IAdminManufacturerModelService, AdminManufacturerModelService>();
        builder.Services.AddScoped<IBaseAdminModelService, BaseAdminModelService>();
        builder.Services.AddScoped<IAccountModelService, AccountModelService>();
        builder.Services.AddScoped<IAddressService, AddressService>();
        builder.Services.AddScoped<IBaseModelService, BaseModelService>();
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

        app.MapControllerRoute(
                    name: "MyArea",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.RegisterRoutes();

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