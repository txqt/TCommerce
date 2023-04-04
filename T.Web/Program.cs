using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;
using System.Text.Json;
using T.Library.Model.JwtToken;
using T.Web.Areas.Services.AccountService;
using T.Web.Areas.Services.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration.GetSection("Url:ApiUrl").Value)
});
builder.Services.AddTransient<IDatabaseControl, DatabaseControl>();
builder.Services.AddTransient<IAccountService, AccountService>();
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
    options.LoginPath = "/account/login/";
    options.AccessDeniedPath = "/";
});


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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
