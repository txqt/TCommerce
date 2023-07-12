using T.Library.Model.Common;
using T.Library.Model.Users;

namespace T.Library.Model.Security
{
    /// <summary>
    /// Represents a permission record-customer role mapping class
    /// </summary>
    public partial class PermissionRecordUserRoleMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the permission record identifier
        /// </summary>
        public int PermissionRecordId { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        public Guid? UserRoleId { get; set; }
        public Role Role { get; set; }
        public PermissionRecord PermissionRecord { get; set; }
    }
}