using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using T.Library.Model.JwtToken;
using T.Library.Model.Users;
using T.WebApi.Extensions;

namespace T.WebApi.Helpers.TokenHelpers
{
    public interface ITokenService
    {
        SigningCredentials GetSigningCredentials(string key);
        Task<List<Claim>> GetClaims(User user);
        JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims);
        //string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string? token);
        Task<string> GenerateRefreshToken();
        Task<string> GenerateAccessToken(User user);
    }
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        //private readonly IConfiguration _jwtSettings;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public TokenService(IConfiguration configuration, UserManager<User> userManager, IOptions<JwtOptions> jwtOptions)
        {
            _configuration = configuration;
            //_jwtSettings = _configuration.GetSection("Authorization");
            _userManager = userManager;
            _jwtOptions = jwtOptions;
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
                new Claim(ClaimTypes.GivenName,user.LastName+" "+ user.FirstName),
                new Claim(ClaimTypes.Email,user.Email ),
                new Claim(ClaimTypes.Role,string.Join(";",roles)),
            };

            return claims;
        }

        public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
            issuer: _jwtOptions.Value.Issuer,
            audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_jwtOptions.Value.AccessTokenExpirationInHours)),
                signingCredentials: signingCredentials);

            return tokenOptions;
        }

        //public string GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[32];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomNumber);
        //        return Convert.ToBase64String(randomNumber);
        //    }
        //}

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string? token)
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

        public async Task<string> GenerateRefreshToken()
        {
            var signingCredentials = GetSigningCredentials(_jwtOptions.Value.RefreshTokenKey);
            var tokenOptions = GenerateTokenOptions(signingCredentials, null);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public async Task<string> GenerateAccessToken(User user)
        {
            var claims = await GetClaims(user);
            var signingCredentials = GetSigningCredentials(_jwtOptions.Value.AccessTokenKey);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
