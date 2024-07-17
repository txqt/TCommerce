using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TCommerce.Services.IRepositoryServices;
using TCommerce.Services.ProductServices;
using TCommerce.Services.ManufacturerServices;
using TCommerce.Services.CategoryServices;
using TCommerce.Services.PictureServices;
using TCommerce.Services.UserServices;
using TCommerce.Services.HomePageServices;
using TCommerce.ServicesSeederService;
using TCommerce.Services.SecurityServices;
using TCommerce.Services.DbManageServices;
using TCommerce.Services.BannerServices;
using TCommerce.Services.UrlRecordServices;
using TCommerce.Services.UserRegistrations;
using TCommerce.Services.ShoppingCartServices;
using TCommerce.Services.CacheServices;
using TCommerce.Services.AddressServices;
using TCommerce.Services.DiscountServices;
using TCommerce.Data;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.Security;
using TCommerce.Web.Attribute;
using TCommerce.Web.IdentityCustoms;
using TCommerce.Core.Models.Options;
using TCommerce.Core.Models.JwtToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using TCommerce.Services.PrepareModelServices.PrepareAdminModel;
using TCommerce.Web.Areas.Admin.Services.PrepareAdminModel;
using TCommerce.Web.Areas.Admin.Services.PrepareModel;
using TCommerce.Web.Common;
using TCommerce.Web.PrepareModelServices;
using TCommerce.Web.Routing;
using TCommerce.Web.Helpers;
using TCommerce.Services.TokenServices;
using TCommerce.Services.PriceCalulationServices;
using TCommerce.Core.Interface;
using TCommerce.Services.PaymentServices;
using TCommerce.Services.OrderServices;
using AspNetCoreRateLimit;
using System.Configuration;
using TCommerce.Services.VNPayServices;

namespace TCommerce.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization(options =>
            {
                // Thêm policy với tên là "Jwt" và yêu cầu tất cả người dùng phải được xác thực bằng JWT
                options.AddPolicy("Jwt", policy =>
                {
                    // Sử dụng scheme xác thực là JwtBearerDefaults.AuthenticationScheme
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    // Yêu cầu tất cả người dùng phải được xác thực
                    policy.RequireAuthenticatedUser();
                });

                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                                        .RequireAuthenticatedUser().Build();
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Authorization:Issuer"],
                    ValidAudience = configuration["Authorization:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authorization:AccessTokenKey"]!)),
                    ClockSkew = TimeSpan.Zero,
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/appHub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
        // Thêm các dịch vụ cần thiết
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(RepositoryService<>));
            services.AddScoped<ICacheService, CacheService>();
            if (!DatabaseManager.IsDatabaseInstalled())
            {
                services.AddScoped<DatabaseManager>();
                //services.AddScoped<DataSeeder>();
                services.AddScoped<DynamicServiceManager>();
                return services;
            }

            //services.AddScoped<IDatabaseControl, DatabaseControl>();
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IProductModelService, ProductModelService>();
            services.AddScoped<ICatalogModelService, CatalogModelService>();
            services.AddScoped<IAdminCategoryModelService, AdminCategoryModelService>();
            services.AddScoped<IAdminProductModelService, AdminProductModelService>();
            services.AddScoped<IAdminBannerModelService, AdminBannerModelService>();
            services.AddScoped<IAdminUserModelService, AdminUserModelService>();
            services.AddScoped<IAdminDiscountModelService, AdminDiscountModelService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBannerService, BannerService>();
            services.AddScoped<IUrlRecordService, UrlRecordService>();
            services.AddScoped<IPictureService, PictureService>();
            services.AddScoped<HttpClientHelper>();
            services.AddScoped<UnauthorizedResponseHandler>();
            services.AddScoped<SlugRouteTransformer>();
            services.AddScoped<IShoppingCartModelService, ShoppingCartModelService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IAdminManufacturerModelService, AdminManufacturerModelService>();
            services.AddScoped<IBaseAdminModelService, BaseAdminModelService>();
            services.AddScoped<IAccountModelService, AccountModelService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IBaseModelService, BaseModelService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IEmailSender, SendMailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPriceCalculationService, PriceCalculationService>();
            services.AddScoped<IProductAttributeConverter, ProductAttributeConverter>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IVNPayService, VNPayService>();
            
            services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            return services;
        }

        // Thêm cấu hình cơ sở dữ liệu
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            if (!DatabaseManager.IsDatabaseInstalled())
                return services;

            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                });

            return services;
        }

        // Thêm cấu hình Identity
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {
            if (!DatabaseManager.IsDatabaseInstalled())
                return services;

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddPasswordValidator<CustomPasswordValidator<User>>()
                .AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation");

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

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(1));

            return services;
        }

        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<UrlOptions>(configuration.GetSection("Url"));
            services.Configure<AuthorizationOptionsConfig>(configuration.GetSection("Authorization"));
            return services;
        }
        public static IServiceCollection AddHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            if (!DatabaseManager.IsDatabaseInstalled())
                return services;

            services.AddScoped<JwtHandler>();
            services.AddHttpClient("", sp =>
            {
                sp.BaseAddress = new Uri(configuration.GetSection("Url:ClientUrl").Value);
            })
                .AddHttpMessageHandler<JwtHandler>()
                .AddHttpMessageHandler<UnauthorizedResponseHandler>();
            return services;
        }
        public static IServiceCollection AddRateLimit(this IServiceCollection services, IConfiguration configuration)
        {
            if (!DatabaseManager.IsDatabaseInstalled())
                return services;

            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            return services;
        }

    }
}
