using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Services.UserService;

namespace T.Web.Services.PrepareModelServices
{
    public interface IUserModelService
    {
        Task<UserViewModel> PrepareUserModelAsync(UserViewModel model, UserModel user);
    }
    public class UserModelService : IUserModelService
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public UserModelService(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
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

            model.AvailableRoles = (await _userService.GetAllRolesAsync()).Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Name
            }).ToList();

            return model;
        }
    }
}
