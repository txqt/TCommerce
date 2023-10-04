using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.JwtToken
{
    public class JwtOptions
    {
        public string AccessTokenKey { get; set; }
        public string RefreshTokenKey { get; set; }
        public int AccessTokenExpirationInHours { get; set; }
        public int AccessTokenExpirationInMinutes { get; set; }
        public int RefreshTokenExpirationInHours { get; set; }
        public int RefreshTokenExpirationInMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
