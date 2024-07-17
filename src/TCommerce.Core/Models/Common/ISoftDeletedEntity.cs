using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Models.Common
{
    public interface ISoftDeletedEntity
    {
        bool Deleted { get; set; }
    }
}
