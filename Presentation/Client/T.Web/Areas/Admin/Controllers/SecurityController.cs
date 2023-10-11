using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;
using T.Web.Attribute;
using T.Web.Controllers;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/security/[action]")]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class SecurityController : BaseController
    {
        private readonly ISecurityService _securityService;
        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new PermissionMappingModel();

            var customerRoles = await _securityService.GetRoles();
            var permissionRecords = await _securityService.GetAllPermissionRecordAsync();
            model.AvailableCustomerRoles = customerRoles;

            foreach (var permissionRecord in permissionRecords)
            {
                model.AvailablePermissions.Add(new PermissionRecord
                {
                    Name = permissionRecord.Name,
                    SystemName = permissionRecord.SystemName
                });

                foreach (var role in customerRoles)
                {
                    if (!model.Allowed.ContainsKey(permissionRecord.SystemName))
                        model.Allowed[permissionRecord.SystemName] = new Dictionary<Guid, bool>();
                    model.Allowed[permissionRecord.SystemName][role.Id] =
                        (await _securityService.GetPermissionMappingAsync(role.Id.ToString(), permissionRecord.Id)).Data != null;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRolePermissions(PermissionMappingModel model)
        {
            // Lấy danh sách tất cả các quyền
            var permissionRecords = await _securityService.GetAllPermissionRecordAsync();
            var roles = await _securityService.GetRoles();

            try
            {
                // Duyệt qua từng quyền
                foreach (var permissionRecord in permissionRecords)
                {
                    // Duyệt qua từng vai trò
                    foreach (var role in roles)
                    {
                        // Kiểm tra xem vai trò có được quyền này không
                        bool hasPermission;
                        if (model.Allowed[permissionRecord.SystemName].TryGetValue(role.Id, out bool value))
                        {
                            hasPermission = value;
                        }
                        else
                        {
                            hasPermission = false;
                        }
                        var mapping = (await _securityService.GetPermissionMappingAsync(role.Id.ToString(), permissionRecord.Id)).Data;


                        // Nếu vai trò có quyền, thêm quyền vào vai trò
                        if (hasPermission)
                        {
                            if (mapping != null)
                            {
                                continue;
                            }

                            var permissionMapping = new PermissionRecordUserRoleMapping
                            {
                                PermissionRecordId = permissionRecord.Id,
                                RoleId = role.Id
                            };
                            await _securityService.CreatePermissionMappingAsync(permissionMapping);
                        }
                        // Nếu vai trò không có quyền, xóa quyền khỏi vai trò
                        else if (mapping != null)
                        {
                            await _securityService.DeletePermissionMappingByIdAsync(mapping.Id);
                        }
                    }
                }
                SetStatusMessage("Đã cập nhật vào hệ thống !");
            }
            catch(Exception ex)
            {
                SetStatusMessage(ex.ToString());
            }
            return RedirectToAction("Index");
        }
    }
}
