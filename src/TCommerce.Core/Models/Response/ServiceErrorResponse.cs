using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Response
{
    public class ServiceErrorResponse<T> : ServiceResponse<T>
    {
        public string[]? ValidationErrors { get; set; }
        public ServiceErrorResponse()
        {

        }
        public ServiceErrorResponse(string message)
        {
            Success = false;
            Message = message;
        }

        public ServiceErrorResponse(string[] validationErrors)
        {
            Success = false;
            ValidationErrors = validationErrors;
        }
    }
}
