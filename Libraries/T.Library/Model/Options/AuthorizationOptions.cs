using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.JwtToken
{
    public class AuthorizationOptions
    {
        public AuthorizationOptions()
        {
        }

        public AuthorizationOptions(string accessTokenKey, string refreshTokenKey, int accessTokenExpirationInSenconds, int refreshTokenExpirationInSenconds, string issuer, string audience)
        {
            AccessTokenKey = accessTokenKey;
            RefreshTokenKey = refreshTokenKey;
            AccessTokenExpirationInSenconds = accessTokenExpirationInSenconds;
            RefreshTokenExpirationInSenconds = refreshTokenExpirationInSenconds;
            Issuer = issuer;
            Audience = audience;
        }

        public string AccessTokenKey { get; set; }
        public string RefreshTokenKey { get; set; }
        public int AccessTokenExpirationInSenconds { get; set; }
        public int RefreshTokenExpirationInSenconds { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
