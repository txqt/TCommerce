using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Security.Claims;
using System.Text;
using T.Library.Model;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Helpers.TokenHelpers;
using T.WebApi.Services.AccountServices;
using T.WebApi.Attribute;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using T.WebApi.Services.CacheServices;
using T.Library.Model.JwtToken;
using T.WebApi.Services;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace T.WebApi.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static object Configuration { get; private set; }

        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TCommerce.Api", Version = "v1" });
                c.AddSecurityDefinition("Bearer", //Name the security scheme
                 new OpenApiSecurityScheme
                 {
                     Description = "JWT Authorization header using the Bearer scheme.",
                     Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
                     Scheme = "bearer" //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                 });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Id = "Bearer", //The name of the previously defined security scheme.
                            Type = ReferenceType.SecurityScheme
                        }
                    },new List<string>()
                }
            });

            });

            return services;
        }

        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            //var _configuration = configuration.GetSection("Authorization");
            //// Lấy khóa bí mật để tạo JWT
            //var key = _configuration["AccessTokenKey"];

            //// Lấy tên người tạo JWT
            //var issuer = _configuration["Issuer"];

            //// Lấy tên đối tượng nhận JWT
            //var audience = _configuration["Audience"];

            //// Lấy thời gian hết hạn của JWT truy cập
            //var accessTokenExpirationInMinutes = _configuration["AccessTokenExpirationInMinutes"];

            var jwtSection = configuration.GetSection("Authorization");
            var jwtOptions = new JwtOptions();
            jwtSection.Bind(jwtOptions);
            services.Configure<JwtOptions>(jwtSection);


            // Cấu hình phân quyền (authorization) sử dụng JWT
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

                // Cấu hình policy mặc định để yêu cầu tất cả các yêu cầu phải được xác thực bằng JWT
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            // Cấu hình xác thực (authentication) sử dụng JWT
            services.AddAuthentication(opt =>
            {
                // Sử dụng scheme xác thực mặc định là JwtBearerDefaults.AuthenticationScheme cho cả việc xác thực và thách thức (challenge)
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

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/appHub"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });


            

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<TokenManagerMiddleware>();
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<IEmailSender, SendMailService>();
            services.AddScoped<ValidationFilterAttribute>();
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }

        public static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {
            services.AddIdentity<User, Library.Model.Users.Role>()
                .AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders()
                .AddDefaultTokenProviders()
                .AddPasswordValidator<CustomPasswordValidator<User>>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                //options.Password.RequireNonLetterOrDigit = true;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });
            return services;
        }

        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var mailSection = configuration.GetSection("MailSettings");
            mailSection.Bind(new MailSettings());
            services.Configure<MailSettings>(mailSection);
            return services;
        }
    }
}
