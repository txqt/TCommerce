using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.RefreshToken
{
    public class AuthResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        //public string? ReturnUrl { get; set; }
    }
}
