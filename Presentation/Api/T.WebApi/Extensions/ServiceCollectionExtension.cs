using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using T.Library.Model.Users;

using T.WebApi.Attribute;
using T.WebApi.Services.CacheServices;
using T.WebApi.Services.ProductServices;
using T.WebApi.Services.CategoryServices;
using T.WebApi.Services.UserServices;
using T.WebApi.Services.HomePageServices;
//using T.WebApi.Services.PermissionRecordUserRoleMappingServices;
using T.WebApi.IdentityCustom;
using T.WebApi.Services.IRepositoryServices;
using T.Library.Model.Interface;
using T.Library.Model.Security;
using T.WebApi.Services.SecurityServices;
using T.Library.Model.Options;
using T.WebApi.Services.DbManageService;
using T.WebApi.Database;
using T.WebApi.Services.BannerServices;
using T.WebApi.Services.PictureServices;
using T.WebApi.Services;
using T.WebApi.ServicesSeederService;
using T.WebApi.Services.TokenServices;
using T.WebApi.Services.UserRegistrations;

namespace T.WebApi.Extensions
{
    public static class ServiceCollectionExtension
    {
        // Thêm Swagger vào dịch vụ
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TCommerce.Api", Version = "v1" });

                // Thêm định nghĩa bảo mật JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                // Yêu cầu bảo mật JWT cho tất cả các tài nguyên
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }

        // Thêm xác thực và phân quyền JWT vào dịch vụ
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authorization:AccessTokenKey"])),
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
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IEmailSender, SendMailService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IManufacturerServicesCommon, ManufacturerServices>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IPictureService, PictureService>();
            services.AddScoped<IUserServiceCommon, UserService>();
            services.AddScoped<IHomePageService, HomePageService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<DataSeeder>();
            services.AddScoped<IDbManageService, DbManageService>();
            services.AddScoped<IBannerService, BannerService>();
            services.AddScoped<ValidationFilterAttribute>();
            services.AddScoped<DatabaseContextFactory>();
            services.AddScoped(typeof(IRepository<>), typeof(RepositoryService<>));
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            return services;
        }

        // Thêm cấu hình cơ sở dữ liệu
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            return services;
        }

        // Thêm cấu hình Identity
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<DatabaseContext>()
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
                options.User.RequireUniqueEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
            });

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2));

            services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromDays(3));

            return services;
        }

        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<UrlOptions>(configuration.GetSection("Url"));
            services.Configure<Library.Model.JwtToken.AuthorizationOptions>(configuration.GetSection("Authorization"));
            return services;
        }


    }
}
