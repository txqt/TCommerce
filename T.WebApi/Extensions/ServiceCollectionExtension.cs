using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Helpers.TokenHelpers;
using T.WebApi.Services.AccountServices;
using T.WebApi.Attribute;
using T.WebApi.Services.CacheServices;
using T.Library.Model.JwtToken;
using T.WebApi.Services.ProductServices;
using T.WebApi.Services.ProductService;
using T.WebApi.Services.CategoryServices;
using T.WebApi.Services.UserServices;
using T.WebApi.Services.HomePageServices;
using T.WebApi.Services.DataSeederService;
using T.WebApi.Services.PermissionRecordServices;
using T.WebApi.Services.PermissionRecordUserRoleMappingServices;
using T.WebApi.IdentityCustom;
using T.WebApi.Middleware.TokenManagers;
using T.WebApi.Middleware.ErrorHandlings;

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
            var jwtSection = configuration.GetSection("Authorization");
            var jwtOptions = new JwtOptions();
            jwtSection.Bind(jwtOptions);
            services.Configure<JwtOptions>(jwtSection);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Jwt", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });

                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
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
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.AccessTokenKey)),
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
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<TokenManagerMiddleware>();
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<IEmailSender, SendMailService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IProductAttributeService, ProductAttributeService>();
            services.AddTransient<IProductAttributeMappingService, ProductAttributeMappingService>();
            services.AddTransient<IProductAttributeValueService, ProductAttributeValueService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IProductCategoryService, ProductCategoryService>();
            services.AddTransient<IPictureService, PictureService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IHomePageService, HomePageService>();
            services.AddTransient<DataSeeder>();
            services.AddTransient<IPermissionRecordService, PermissionRecordService>();
            services.AddTransient<IPermissionRecordUserRoleMappingService, PermissionRecordUserRoleMappingService>();
            services.AddScoped<ValidationFilterAttribute>();

            return services;
        }

        // Thêm cấu hình cơ sở dữ liệu
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }

        // Thêm cấu hình Identity
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders()
                .AddPasswordValidator<CustomPasswordValidator<User>>();

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
            });

            return services;
        }

        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            return services;
        }


    }
}
