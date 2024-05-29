using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.ViewsModel
{
    public class AddressInfoModel : BaseEntity
    {
        public string? FullName { get; set; }
        public string? AddressFull { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsDefault { get; set; }
    }
}
