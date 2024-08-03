﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Web.Areas.Admin.Models.Datatables;
using TCommerce.Web.Areas.Admin.Models.Users;
using TCommerce.Web.Areas.Admin.Services.PrepareAdminModel;
using TCommerce.Web.Attribute;
using TCommerce.Web.Extensions;

namespace TCommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/user/[action]")]
    [CheckPermission(PermissionSystemName.ManageUsers)]
    public class UserController : BaseAdminController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IAdminUserModelService _prepareModelService;

        public UserController(IUserService userService, IMapper mapper, IAdminUserModelService prepareModelService)
        {
            _userService = userService;
            _mapper = mapper;
            _prepareModelService = prepareModelService;
        }

        public IActionResult Index()
        {
            var model = new DataTableViewModel
            {
                TableTitle = "Danh sách User",
                CreateUrl = Url.Action("Create", "User"),
                EditUrl = Url.Action("Edit", "User"),
                DeleteUrl = Url.Action("DeleteUser", "User"),
                GetDataUrl = Url.Action("GetAll", "User"),
                Columns = new List<ColumnDefinition>
                {
                    new ColumnDefinition(nameof(UserModel.FirstName)) { Title = DisplayNameExtensions.GetPropertyDisplayName<UserModel>(m=>m.FirstName) },
                    new ColumnDefinition(nameof(UserModel.LastName)) { Title = DisplayNameExtensions.GetPropertyDisplayName<UserModel>(m=>m.LastName) },
                    new ColumnDefinition(nameof(UserModel.Email)) { Title = DisplayNameExtensions.GetPropertyDisplayName<UserModel>(m=>m.Email) },
                    new ColumnDefinition(nameof(UserModel.UserName)) { Title = DisplayNameExtensions.GetPropertyDisplayName<UserModel>(m=>m.UserName) },
                    new ColumnDefinition(nameof(UserModel.PhoneNumber)) { Title = DisplayNameExtensions.GetPropertyDisplayName<UserModel>(m=>m.PhoneNumber) },
                    new ColumnDefinition(nameof(UserModel.Deleted)) { RenderType = RenderType.RenderBoolean, Title = DisplayNameExtensions.GetPropertyDisplayName<UserModel>(m=>m.Deleted) },
                    new ColumnDefinition(nameof(UserModel.Id)) { RenderType = RenderType.RenderButtonEdit },
                    new ColumnDefinition(nameof(UserModel.Id)) { RenderType = RenderType.RenderButtonRemove },
                }
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var userList = await _userService.GetAllAsync();

            var models = _mapper.Map<List<UserModel>>(userList);

            return this.JsonWithPascalCase(models);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _prepareModelService.PrepareUserModelAsync(new UserModel(), null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _prepareModelService.PrepareUserModelAsync(model, null);
                return View(model);
            }

            var userModel = _mapper.Map<User>(model);

            var result = await _userService.CreateUserAsync(userModel, model.RoleIds, model.Password);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                model = await _prepareModelService.PrepareUserModelAsync(model, null);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = (await _userService.GetUserById(id)) ??
                throw new ArgumentException("No user found with the specified id");

            var model = await _prepareModelService.PrepareUserModelAsync(new UserModel(), user);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _prepareModelService.PrepareUserModelAsync(model, null);
                return View(model);
            }

            var user = (await _userService.GetUserById(model.Id)) ??
                throw new ArgumentException("No user found with the specified id");

            _mapper.Map(model, user);

            var result = await _userService.UpdateUserAsync(user, model.RoleIds, model.Password);
            
            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareUserModelAsync(model, user);
                return View(model);
            }

            SetStatusMessage("Sửa thành công");
            model = await _prepareModelService.PrepareUserModelAsync(model, user);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(Guid id)
        {

            var result = await _userService.DeleteUserByUserIdAsync(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }

        [HttpDelete]
        public async Task<IActionResult> BanUser(Guid id)
        {

            var result = await _userService.BanUser(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
    }
}