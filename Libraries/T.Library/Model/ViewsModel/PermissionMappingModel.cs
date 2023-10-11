using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;
using T.Library.Model.Security;

namespace T.Library.Model.ViewsModel
{
    public class PermissionMappingModel : BaseEntity
    {
        #region Ctor

        public PermissionMappingModel()
        {
            AvailablePermissions = new List<PermissionRecord>();
            AvailableCustomerRoles = new List<Role>();
            Allowed = new Dictionary<string, IDictionary<Guid, bool>>();
        }

        #endregion

        #region Properties

        public IList<PermissionRecord> AvailablePermissions { get; set; }

        public IList<Role> AvailableCustomerRoles { get; set; }

        //[permission system name] / [customer role id] / [allowed]
        public IDictionary<string, IDictionary<Guid, bool>> Allowed { get; set; }

        #endregion
    }
}
