using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Accounts;
using TCommerce.Core.Models.RefreshToken;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Users;

namespace TCommerce.Core.Interface
{
    public interface ITokenService
    {
        Task<ServiceResponse<AuthResponseDto>> Create(User user);
        Task<ServiceResponse<AuthResponseDto>> RefreshToken(RefreshTokenRequestModel tokenDto);
        Task<string> GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalToken(string? token);
    }
}
