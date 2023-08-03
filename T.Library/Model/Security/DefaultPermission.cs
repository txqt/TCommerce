using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T.Library.Model.Security
{
    public class DefaultPermission
    {
        public static readonly PermissionRecord AccessAdminPanel = new() { Name = "Access admin area", SystemName = "AccessAdminPanel", Category = "Manager" };
        public static readonly PermissionRecord ManageProducts = new() { Name = "Admin area: Manage Products", SystemName = "ManageProducts", Category = "Manager" };
        public static readonly PermissionRecord ManageCategories = new() { Name = "Admin area: Manage Categories", SystemName = "ManageCategories", Category = "Manager" };
        public static readonly PermissionRecord ManageAttributes = new() { Name = "Admin area: Manage Attributes", SystemName = "ManageAttributes", Category = "Manager" };
        public static readonly PermissionRecord ManageCustomers = new() { Name = "Admin area: Manage Users", SystemName = "ManageCustomers", Category = "Manager" };
    }
}
