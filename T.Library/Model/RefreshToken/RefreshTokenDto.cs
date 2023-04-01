using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.RefreshToken
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string ReturnUrl { get; set; }
    }
}
