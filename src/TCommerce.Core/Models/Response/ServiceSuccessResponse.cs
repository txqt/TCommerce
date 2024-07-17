using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Response
{
    public class ServiceSuccessResponse<T> : ServiceResponse<T>
    {
        public ServiceSuccessResponse(T data)
        {
            Success = true;
            Data = data;
        }

        public ServiceSuccessResponse()
        {
            Success = true;
        }
    }
}
