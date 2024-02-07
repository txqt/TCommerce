using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model.Interface;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Services.UserService;

namespace T.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminUserModelService
    {
        Task<UserViewModel> PrepareUserModelAsync(UserViewModel model, UserModel user);
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

        public async Task<UserViewModel> PrepareUserModelAsync(UserViewModel model, UserModel user)
        {
            if (user is not null)
            {
                model ??= new UserViewModel()
                {
                    Id = user.Id
                };
                _mapper.Map(user, model);
            }

            model.AvailableRoles = (await _securityService.GetRoles()).Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Name
            }).ToList();

            return model;
        }
    }
}
