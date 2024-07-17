using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Response
{
    public class LoginResponse<T> : ServiceResponse<T>
    {
        public List<string>? RoleNames { get; set; }
        public LoginResponse(T data)
        {
            Data = data;
        }
        public LoginResponse()
        {
        }


    }
}
