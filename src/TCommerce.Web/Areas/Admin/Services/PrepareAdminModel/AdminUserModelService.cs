﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Utilities;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Roles;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Services.CategoryServices;
using TCommerce.Web.Areas.Admin.Models.Users;
using TCommerce.Web.Models.Catalog;

namespace TCommerce.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminUserModelService
    {
        Task<UserModel> PrepareUserModelAsync(UserModel model, User user);
        Task<UserSearchModel> PrepareUserSearchModel(UserSearchModel model);
    }
    public class AdminUserModelService : IAdminUserModelService
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ISecurityService _securityService;
        public AdminUserModelService(IMapper mapper, IUserService userService, ISecurityService securityService)
        {
            _mapper = mapper;
            _userService = userService;
            _securityService = securityService;
        }

        public async Task<UserModel> PrepareUserModelAsync(UserModel model, User user)
        {
            model.AvailableRoles = (await _securityService.GetRoles()).Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString()
            }).ToList();

            if (user is not null)
            {
                model ??= new UserModel
                {
                    Id = user.Id
                };

                _mapper.Map(user, model);

                // Lấy danh sách vai trò của người dùng
                var roles = await _userService.GetRolesByUserAsync(user);

                // Gán các vai trò đã chọn vào model.RoleIds
                model.RoleIds = roles.Select(r => r.Id).ToList();

                // Đánh dấu các vai trò đã chọn
                foreach (var role in model.AvailableRoles)
                {
                    if (model.RoleIds.Contains(Guid.Parse(role.Value)))
                    {
                        role.Selected = true;
                    }
                }
            }

            return model;
        }

        public async Task<UserSearchModel> PrepareUserSearchModel(UserSearchModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var allRoles = await _securityService.GetRoles();

            model.AvailableUserRoles = (allRoles).Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString()
            }).ToList();

            model.SelectedUserRoleIds.Add(allRoles.Where(x => x.Name == RoleName.Registerd).FirstOrDefault().Id);

            return model;
        }
    }
}
