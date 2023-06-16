using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Enum;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Attribute;
using T.Web.Controllers;
using T.Web.Services.PrepareModel;
using T.Web.Services.UserService;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/user/[action]")]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IPrepareModelService _prepareModelService;

        public UserController(IUserService userService, IMapper mapper, IPrepareModelService prepareModelService)
        {
            _userService = userService;
            _mapper = mapper;
            _prepareModelService = prepareModelService;
        }

        public IActionResult Index()
        {
            return View(new UserModel());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var userList = await _userService.GetAllAsync();

            return Json(new { data = userList });
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

            var result = await _userService.CreateOrEditAsync(userModel);

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
            var user = (await _userService.Get(id)).Data ??
                throw new ArgumentException("No user found with the specified id");

            var model = await _prepareModelService.PrepareUserModelAsync(new UserViewModel(), user);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _prepareModelService.PrepareUserModelAsync(new UserViewModel(), null);
                return View(model);
            }

            var user = (await _userService.Get(model.Id)).Data ??
                throw new ArgumentException("No user found with the specified id");

            user = _mapper.Map(model, user);

            var result = await _userService.CreateOrEditAsync(user);
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

            var result = await _userService.DeleteAsync(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
    }
}
