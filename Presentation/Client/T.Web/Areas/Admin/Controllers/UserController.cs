using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Areas.Admin.Services.PrepareAdminModel;
using T.Web.Attribute;
using T.Web.Extensions;
using T.Web.Services.PrepareModelServices.PrepareAdminModel;
using T.Web.Services.UserService;

namespace T.Web.Areas.Admin.Controllers
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

            var model = new { data = userList };

            return this.JsonWithPascalCase(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _prepareModelService.PrepareUserModelAsync(new UserViewModel(), null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _prepareModelService.PrepareUserModelAsync(new UserViewModel(), null);
                return View(model);
            }

            var userModel = _mapper.Map<UserModel>(model);

            var result = await _userService.CreateUserAsync(userModel);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = (await _userService.Get(id)) ??
                throw new ArgumentException("No user found with the specified id");

            var model = await _prepareModelService.PrepareUserModelAsync(new UserViewModel(), user);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //model = await _prepareModelService.PrepareUserModelAsync(new UserViewModel(), null);
                return View(model);
            }

            var user = (await _userService.Get(model.Id)) ??
                throw new ArgumentException("No user found with the specified id");

            user = _mapper.Map(model, user);

            var result = await _userService.UpdateUserAsync(user);
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

            var result = await _userService.BanUser(id.ToString());
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
    }
}
