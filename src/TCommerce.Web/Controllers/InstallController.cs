using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Startup;
using TCommerce.Core.Models.Users;
using TCommerce.Data;
using TCommerce.Services.AddressServices;
using TCommerce.Services.CacheServices;
using TCommerce.Services.CategoryServices;
using TCommerce.Services.DbManageServices;
using TCommerce.Services.DiscountServices;
using TCommerce.Services.IRepositoryServices;
using TCommerce.Services.ManufacturerServices;
using TCommerce.Services.ProductServices;
using TCommerce.Services.SecurityServices;
using TCommerce.Services.ShoppingCartServices;
using TCommerce.Services.UrlRecordServices;
using TCommerce.Services.UserServices;
using TCommerce.ServicesSeederService;

namespace TCommerce.Web.Controllers
{
    public class InstallController : BaseController
    {
        private IServiceProvider _serviceProvider;
        private IHostEnvironment _hostEnvironment;
        public InstallController(IServiceProvider serviceProvider, IHostEnvironment hostEnvironment)
        {
            _serviceProvider = serviceProvider;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View(new StartupFormModel());
        }

        [HttpPost()]
        public async Task<IActionResult> Install(StartupFormModel model)
        {

            string connectionString = DatabaseManager.BuildConnectionString(model.ServerName, model.DbName,
                model.SqlUsername, model.SqlPassword, model.UseWindowsAuth);

            string connectionStringKey = "ConnectionStrings:DefaultConnection";

            if (AppSettingsExtensions.GetKey(connectionStringKey) is null)
            {
                AppSettingsExtensions.CreateKey(connectionStringKey);
                throw new Exception("Some resources were lost but may have been fixed, please try again");
            }
            AppSettingsExtensions.AddToKey(connectionStringKey, connectionString);

            if (model.CreateDatabaseIfNotExist)
            {
                await DatabaseManager.CreateDatabaseAsync();
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var services = new ServiceCollection();
            var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
            IConfiguration configuration = configurationBuilder.Build();

            services.AddSingleton(configuration);
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(AppSettingsExtensions.GetKey("ConnectionStrings:DefaultConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 3;
                //options.User.RequireUniqueEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
            });
            services.AddScoped(typeof(IRepository<>), typeof(RepositoryService<>));
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IProductAttributeConverter, ProductAttributeConverter>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailSender, SendMailService>();
            services.AddScoped<IUrlRecordService, UrlRecordService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<DatabaseManager>();
            services.AddScoped<DataSeeder>();
            services.AddAutoMapper(typeof(Program).Assembly);

            _serviceProvider = services.BuildServiceProvider();

            using var scope = _serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var databaseManager = scopedServices.GetRequiredService<DatabaseManager>();
            await databaseManager.InitializeTables();

            var dataSeeder = scopedServices.GetRequiredService<DataSeeder>();
            await dataSeeder.Initialize(model.AdminEmail, model.AdminPassword, model.CreateSampleData);


            return Json(new { success = true });
        }

    }
}
