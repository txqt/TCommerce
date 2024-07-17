using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Accounts;
using TCommerce.Core.Models.JwtToken;
using TCommerce.Core.Models.Options;
using TCommerce.Core.Models.RefreshToken;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.SendMail;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Utilities;

namespace TCommerce.Services.TokenServices
{
    public class TokenService : ITokenService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailService;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<AuthorizationOptionsConfig> _jwtOptions;
        private readonly IOptions<UrlOptions> _urlOptions;

        public TokenService(UserManager<User> userManager,
                               SignInManager<User> signInManager,
                               IEmailSender emailService,
                               IOptions<AuthorizationOptionsConfig> jwtOptions,
                               IOptions<UrlOptions> urlOptions)
        {
            this._emailService = emailService;
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions;
            _urlOptions = urlOptions;
        }



        public async Task<ServiceResponse<AuthResponseDto>> Create(User user)
        {

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
            ArgumentNullException.ThrowIfNull(user);

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? "undefined"),
                new Claim(ClaimTypes.GivenName,user.FirstName +" "+ user.LastName),
                new Claim(ClaimTypes.Email,user.Email ?? "undefined")
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }

        public async Task<string> GenerateAccessToken(User user)
        {
            var claims = await GetClaims(user);
            var signingCredentials = GetSigningCredentials(_jwtOptions.Value.AccessTokenKey ?? "");
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(_jwtOptions.Value.AccessTokenExpirationInSenconds),
                signingCredentials: signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public string GenerateRefreshToken()
        {
            var signingCredentials = GetSigningCredentials(_jwtOptions.Value.RefreshTokenKey ?? "");
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.AccessTokenKey ?? "")),
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
        private async Task<ServiceResponse<AuthResponseDto>> CreateNecessaryToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            //Create token
            var accessToken = await GenerateAccessToken(user);

            //Create refresh token
            var refreshToken = GenerateRefreshToken();

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
