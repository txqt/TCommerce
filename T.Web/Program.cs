using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;
using System.Text.Json;
using T.Library.Model.JwtToken;
using T.Web.Attribute;
using T.Web.CusomMiddleware;
using T.Web.Services.AccountService;
using T.Web.Services.CategoryService;
using T.Web.Services.Database;
using T.Web.Services.PrepareModel;
using T.Web.Services.ProductService;
using T.Web.Services.UserService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration.GetSection("Url:ApiUrl").Value)
});
builder.Services.AddTransient<IDatabaseControl, DatabaseControl>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IProductAttributeService, ProductAttributeService>();
builder.Services.AddTransient<IProductAttributeMappingService, ProductAttributeMappingService>();
builder.Services.AddTransient<IProductAttributeValueService, ProductAttributeValueService>();
builder.Services.AddTransient<IPrepareModelService, PrepareModelService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IProductCategoryService, ProductCategoryService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddSingleton<JsonSerializerOptions>(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    IgnoreNullValues = true
});


var jwtSection = builder.Configuration.GetSection("Authorization");
var jwtOptions = new JwtOptions();
jwtSection.Bind(jwtOptions);
builder.Services.Configure<JwtOptions>(jwtSection);


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(jwtOptions.AccessTokenExpirationInHours);
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
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
