using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;
using T.Library.Model.Users;

namespace T.Library.Model.Interface
{
    public interface ITokenServiceCommon
    {
        Task<ServiceResponse<AuthResponseDto>> Create(AccessTokenRequestModel model);
        Task<ServiceResponse<AuthResponseDto>> RefreshToken(RefreshTokenRequestModel tokenDto);
    }
}
