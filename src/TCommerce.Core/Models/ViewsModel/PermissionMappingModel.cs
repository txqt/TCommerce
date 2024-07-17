using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Security;

namespace TCommerce.Core.Models.ViewsModel
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
