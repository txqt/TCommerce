using App.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using T.Library.Model;
using T.Library.Model.Account;
using T.Library.Model.Interface;
using T.Library.Model.JwtToken;
using T.Library.Model.Options;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.SendMail;
using T.Library.Model.Users;
using T.WebApi.Database;
using T.WebApi.Extensions;
using T.WebApi.Services.CacheServices;
using T.WebApi.Services.UserRegistrations;

namespace T.WebApi.Services.TokenServices
{
    public interface ITokenService : ITokenServiceCommon
    {
        Task<string> GenerateAccessToken(User user);
        Task<string> GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalToken(string? token);
    }
    public class TokenService : ITokenService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailService;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<AuthorizationOptionsConfig> _jwtOptions;
        private readonly IOptions<UrlOptions> _urlOptions;
        private readonly IUserRegistrationService _userRegistrationService;

        public TokenService(UserManager<User> userManager,
                               SignInManager<User> signInManager,
                               IEmailSender emailService,
                               IOptions<AuthorizationOptionsConfig> jwtOptions,
                               IOptions<UrlOptions> urlOptions,
                               IUserRegistrationService userRegistrationService)
        {
            this._emailService = emailService;
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions;
            _urlOptions = urlOptions;
            _userRegistrationService = userRegistrationService;
        }



        public async Task<ServiceResponse<AuthResponseDto>> Create(AccessTokenRequestModel login)
        {
            if (login.UserNameOrEmail is null || login.Password is null)
                throw new ArgumentNullException("Must enter the necessary information");

            var user = await _userManager.FindByNameAsync(login.UserNameOrEmail);

            if (user is null && AppUtilities.IsValidEmail(login.UserNameOrEmail))
            {
                user = await _userManager.FindByEmailAsync(login.UserNameOrEmail);
            }

            if (user == null)
            {
                // Người dùng không tồn tại
                return new ServiceErrorResponse<AuthResponseDto>() { Data = null, Message = "User is not found"};
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, login.RememberMe, lockoutOnFailure: true);

            if (result.IsNotAllowed)
                return await HandleNotAllowedUser(user);

            if (user.RequirePasswordChange)
                return await HandlePasswordChangeRequired(user);

            if (result.IsLockedOut)
                return HandleLockedOutUser(user);

            if (!result.Succeeded)
                return new ServiceResponse<AuthResponseDto>() { Message = "Tài khoản hoặc mật khẩu không đúng", Success = false };

            return await CreateNecessaryToken(user);
        }

        public async Task<ServiceResponse<AuthResponseDto>> RefreshToken(RefreshTokenRequestModel tokenDto)
        {
            if (tokenDto is null)
            {
                return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "TokenDto is null" };
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == tokenDto.RefreshToken);

            if (user == null)
                return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "User not found" };
            if (user.RefreshToken != tokenDto.RefreshToken)
                return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "Invalid refresh token" };
            if (user.RefreshTokenExpiryTime <= DateTime.Now)
                return new ServiceErrorResponse<AuthResponseDto> { Success = false, Message = "Refresh token expired" };

            var accessToken = await GenerateAccessToken(user);

            var data = new AuthResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = tokenDto.RefreshToken
            };

            return new ServiceErrorResponse<AuthResponseDto> { Success = true, Data = data };
        }

        public SigningCredentials GetSigningCredentials(string _key)
        {
            var key = Encoding.UTF8.GetBytes(_key);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<List<Claim>> GetClaims(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName,user.FirstName+" "+ user.LastName),
                new Claim(ClaimTypes.Email,user.Email )
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }

        public async Task<string> GenerateAccessToken(User user)
        {
            var claims = await GetClaims(user);
            var signingCredentials = GetSigningCredentials(_jwtOptions.Value.AccessTokenKey);
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(_jwtOptions.Value.AccessTokenExpirationInSenconds),
                signingCredentials: signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public async Task<string> GenerateRefreshToken()
        {
            var signingCredentials = GetSigningCredentials(_jwtOptions.Value.RefreshTokenKey);
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                expires: DateTime.UtcNow.AddSeconds(_jwtOptions.Value.RefreshTokenExpirationInSenconds),
                signingCredentials: signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public ClaimsPrincipal GetPrincipalToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Value.Issuer,
                ValidAudience = _jwtOptions.Value.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.AccessTokenKey)),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }


        #region private methods
        private async Task<ServiceErrorResponse<AuthResponseDto>> HandleNotAllowedUser(User user)
        {
            var message = "Tài khoản của bạn đã bị chặn và không thể đăng nhập vào hệ thống";
            if (user.EmailConfirmed == false && user.Email is not null)
            {
                message = await SendConfirmationEmail(user);
            }
            return new ServiceErrorResponse<AuthResponseDto>() { Message = $"{message}" };
        }
        private async Task<string> SendConfirmationEmail(User user)
        {
            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodeToken = EncodeToken(confirmEmailToken);

            string url = $"{_urlOptions.Value.APIUrl}/api/user/confirm-email?userid={user.Id}&token={encodeToken}";

            if (user.Email is null)
                return "Không thể tìm thấy email người dùng";

            EmailDto emailDto = new EmailDto
            {
                Subject = "Xác thực email người dùng",
                Body = $"<h1>Xin chào, {user.LastName + " " + user.FirstName}</h1><br/>"
                + $"<h3>Tài khoản: {user.UserName}</h3></br>"
                + $"<p>Hãy xác nhận email của bạn <a href='{url}'>Bấm vào đây</a></p>",
                To = user.Email
            };

            try
            {
                await _emailService.SendEmailAsync(emailDto);
                return "Tài khoản của bạn chưa được xác thực. Hệ thống đã gửi email đến cho bạn, vui lòng kiểm tra email của bạn và làm theo hướng dẫn để hoàn tất quá trình xác thực.\r\n";
            }
            catch
            {
                return "Tài khoản của bạn chưa được xác thực. Không thể gửi email xác nhận, vui lòng thử lại hoặc liên hệ bộ phận kỹ thuật";
            }
        }
        private async Task<ServiceResponse<AuthResponseDto>> HandlePasswordChangeRequired(User user)
        {
            if (user.Email is null)
                return new ServiceErrorResponse<AuthResponseDto>() { Message = "Không thể tìm thấy email người dùng" };

            await _userRegistrationService.SendResetPasswordEmail(user.Email);

            return new ServiceErrorResponse<AuthResponseDto>() { Message = "Tài khoản của bạn cần đổi mật khẩu, vui lòng kiểm tra mail" };
        }
        private ServiceResponse<AuthResponseDto> HandleLockedOutUser(User user)
        {
            var timeLockOutEnd = DateTimeOffset.Now - user.LockoutEnd;

            if (timeLockOutEnd is null)
            {
                return new ServiceErrorResponse<AuthResponseDto>() { Message = $"Something went wrong", Success = false };
            }

            return new ServiceErrorResponse<AuthResponseDto>() { Message = $"Thử lại sau {timeLockOutEnd.Value}", Success = false };
        }
        private async Task<ServiceResponse<AuthResponseDto>> CreateNecessaryToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            //Create token
            var accessToken = await GenerateAccessToken(user);

            //Create refresh token
            var refreshToken = await GenerateRefreshToken();

            //Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddSeconds(_jwtOptions.Value.RefreshTokenExpirationInSenconds);
            await _userManager.UpdateAsync(user);

            var response = new AuthResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            return new LoginResponse<AuthResponseDto>() { Success = true, Data = response, RoleNames = roles.ToList() };
        }
        private string EncodeToken(string normalToken)
        {
            var encodedEmailToken = Encoding.UTF8.GetBytes(normalToken);
            return WebEncoders.Base64UrlEncode(encodedEmailToken);
        }
        #endregion
    }
}
