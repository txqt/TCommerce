using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Options
{
    public class MomoOptions
    {
        public string MomoApiUrl { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
        public string NotifyUrl { get; set; } = null!;
        public string PartnerCode { get; set; } = null!;
        public string RequestType { get; set; } = null!;
    }
}
