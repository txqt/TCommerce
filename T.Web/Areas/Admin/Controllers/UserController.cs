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
    [Route("/admin/category/[action]")]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class UserController : BaseController
    {
        //private readonly IUserService _userService;
        //private readonly IMapper _mapper;
        //private readonly IPrepareModelService _prepareModelService;

        //public UserController(IUserService userService, IMapper mapper, IPrepareModelService prepareModelService)
        //{
        //    _userService = userService;
        //    _mapper = mapper;
        //    _prepareModelService = prepareModelService;
        //}

        //public IActionResult Index()
        //{
        //    return View(new CategoryModel());
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetAllAsync()
        //{
        //    var categoryList = await _userService.GetAllAsync();

        //    var listModel = _mapper.Map<List<CategoryModel>>(categoryList);
        //    foreach (var item in listModel)
        //    {
        //        if (item.ParentCategoryId > 0)
        //        {
        //            item.ParentCategoryName = (await _userService.Get(item.ParentCategoryId)).Data.Name;
        //        }
        //    }

        //    return Json(new { data = listModel });
        //}

        //[HttpGet]
        //public async Task<IActionResult> Create()
        //{
        //    var model = await _prepareModelService.PrepareCategoryModelAsync(new CategoryModel(), null);

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(UserModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var result = await _userService.CreateOrEditAsync(model);

        //    if (!result.Success)
        //    {
        //        ModelState.AddModelError(string.Empty, result.Message);
        //        return View(model);
        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var category = (await _userService.Get(id)).Data ??
        //        throw new ArgumentException("No category found with the specified id");

        //    var model = await _prepareModelService.PrepareCategoryModelAsync(new CategoryModel(), category);

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(CategoryModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var category = (await _userService.Get(model.Id)).Data ??
        //        throw new ArgumentException("No category found with the specified id");

        //    category = _mapper.Map(model, category);

        //    var result = await _userService.AddOrEdit(category);
        //    if (!result.Success)
        //    {
        //        SetStatusMessage($"{result.Message}");
        //        model = await _prepareModelService.PrepareCategoryModelAsync(model, category);
        //        return View(model);
        //    }

        //    SetStatusMessage("Sửa thành công");
        //    model = await _prepareModelService.PrepareCategoryModelAsync(model, category);

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> DeleteCategory(int id)
        //{

        //    var result = await _userService.Delete(id);
        //    if (!result.Success)
        //    {
        //        return Json(new { success = false, message = result.Message });
        //    }
        //    return Json(new { success = true, message = result.Message });
        //}
    }
}
