using T.Library.Model.Common;

namespace T.Library.Model.Security
{
    /// <summary>
    /// Represents a permission record
    /// </summary>
    public partial class PermissionRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the permission system name
        /// </summary>
        public string SystemName { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the permission category
        /// </summary>
        public string? Category { get; set; }
        
        public List<PermissionRecordUserRoleMapping>? PermissionRecordUserRoleMappings { get; set; }
    }
}