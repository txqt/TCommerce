using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TCommerce.Core.Extensions;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Services.OrderServices;
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
        private readonly RoleManager<Role> _roleManager;

        public UserController(IUserService userService, IMapper mapper, IAdminUserModelService prepareModelService, RoleManager<Role> roleManager)
        {
            _userService = userService;
            _mapper = mapper;
            _prepareModelService = prepareModelService;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _prepareModelService.PrepareUserSearchModel(new UserSearchModel()));
        }

        [HttpPost]
        public async Task<IActionResult> GetAllAsync(UserSearchModel userSearchModel)
        {
            var userParameters = ParseQueryStringParameters<UserParameters>();
            _mapper.Map(userSearchModel, userParameters);
            if(userSearchModel.SelectedUserRoleIds is not null && userSearchModel.SelectedUserRoleIds.Any())
            {
                foreach(var srId in userSearchModel.SelectedUserRoleIds)
                {
                    var roleName = (await _roleManager.FindByIdAsync(srId.ToString())).Name;
                    if(!string.IsNullOrEmpty(roleName))
                        userParameters.Roles.Add(roleName);
                }
            }

            var response = await _userService.GetAllUsersAsync(userParameters);

            var pagingResponse = new PagingResponse<User>
            {
                Items = response,
                MetaData = response.MetaData
            };

            var model = ToDatatableReponse(pagingResponse.MetaData.TotalCount, pagingResponse.MetaData.TotalCount, pagingResponse.Items);

            return this.JsonWithPascalCase(model);
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

        public virtual async Task<IActionResult> LoadCustomerStatistics(string period)
        {
            var result = new List<object>();

            var nowDt = DateTime.UtcNow.ConvertToUserTime(DateTimeExtensions.GetCurrentTimeZone());
            var timeZone = DateTimeExtensions.GetCurrentTimeZone();

            switch (period)
            {
                case "year":
                    // year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (var i = 0; i <= 12; i++)
                    {
                        result.Add(new
                        {
                            date = searchYearDateUser.Date.ToString("Y", CultureInfo.CurrentCulture),
                            value = (await _userService.GetAllUsersAsync(new UserParameters()
                            {
                                CreatedFromUtc = searchYearDateUser.ConvertToUtcTime(timeZone),
                                CreatedToUtc = searchYearDateUser.AddMonths(1).ConvertToUtcTime(timeZone),
                                PageNumber = 1,
                                PageSize = 1
                            })).MetaData.TotalCount.ToString()
                        });

                        searchYearDateUser = searchYearDateUser.AddMonths(1);
                    }

                    break;
                case "month":
                    // month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (var i = 0; i <= 30; i++)
                    {
                        result.Add(new
                        {
                            date = searchMonthDateUser.Date.ToString("M", CultureInfo.CurrentCulture),
                            value = (await _userService.GetAllUsersAsync(new UserParameters()
                            {
                                CreatedFromUtc = searchMonthDateUser.ConvertToUtcTime(timeZone),
                                CreatedToUtc = searchMonthDateUser.AddDays(1).ConvertToUtcTime(timeZone),
                                PageNumber = 1,
                                PageSize = 1
                            })).MetaData.TotalCount.ToString()
                        });

                        searchMonthDateUser = searchMonthDateUser.AddDays(1);
                    }

                    break;
                case "week":
                default:
                    // week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        result.Add(new
                        {
                            date = searchWeekDateUser.Date.ToString("d dddd", CultureInfo.CurrentCulture),
                            value = (await _userService.GetAllUsersAsync(new UserParameters()
                            {
                                CreatedFromUtc = searchWeekDateUser.ConvertToUtcTime(timeZone),
                                CreatedToUtc = searchWeekDateUser.AddDays(1).ConvertToUtcTime(timeZone),
                                PageNumber = 1,
                                PageSize = 1
                            })).MetaData.TotalCount.ToString()
                        });

                        searchWeekDateUser = searchWeekDateUser.AddDays(1);
                    }

                    break;
            }

            return Json(result);
        }
    }
}
