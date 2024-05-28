using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.Orders
{
    public enum OrderStatus
    {
        Pending = 1,

        Processing = 2,

        Complete = 3,

        Cancelled = 4
    }
}
