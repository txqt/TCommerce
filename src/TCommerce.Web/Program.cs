using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.Json;
using System.Text.Json.Serialization;
using TCommerce.Core.Extensions;
using TCommerce.Core.Models.JwtToken;
using TCommerce.Web.Extensions;
using TCommerce.Web.Routing;
using TCommerce.Web.Middleware;
using AspNetCoreRateLimit;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(
                                 new SlugifyParameterTransformer()));
})
    .AddDataAnnotationsLocalization().AddRazorRuntimeCompilation().AddJsonOptions(options =>
    {
        //options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddServices();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddHttpClient(builder.Configuration);
builder.Services.AddIdentityConfig();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddCustomOptions(builder.Configuration);
builder.Services.AddRateLimit(builder.Configuration);
builder.Services.AddSetting();
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
    options.LoginPath = "/sign-in-sign-up";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/AccessDenied";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
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

if (app.Environment.IsDevelopment())
{
    app.ConfigureCustomMiddleware();
}



app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.RegisterRoutes();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();