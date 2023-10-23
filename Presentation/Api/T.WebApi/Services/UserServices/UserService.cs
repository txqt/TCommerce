using T.Library.Model.Response;
using T.Library.Model;
using T.Library.Model.Users;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using T.WebApi.Extensions;
using Microsoft.AspNetCore.Identity;
using T.Library.Model.ViewsModel;
using App.Utilities;
using System.Text;
using T.Library.Model.Security;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.WebApi.Database;

namespace T.WebApi.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IMapper mapper, DatabaseContext context, UserManager<User> userManager, RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<bool>> CreateUserAsync(UserModel model)
        {
            if (model.Email is not null)
            {
                if (!AppUtilities.IsValidEmail(model.Email))
                    return new ServiceErrorResponse<bool>("Cần nhập đúng định dạng email");

                if ((await _userManager.FindByEmailAsync(model.Email) != null))
                {
                    return new ServiceErrorResponse<bool>("Email đã tồn tại");
                }
            }

            if ((await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber) != null))
                return new ServiceErrorResponse<bool>("Số điện thoại đã được đăng ký");

            model.Password = model.ConfirmPassword = GenerateRandomPassword(length: 6);

            if(model.UserName is not null && AppUtilities.IsValidEmail(model.UserName)){
                return new ServiceErrorResponse<bool>("Username không thể là 1 email");
            }

            var user = _mapper.Map<User>(model);

            var result = await _userManager.CreateAsync(user, model.Password);

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (model.RoleNames != null && model.RoleNames.Any())
            {
                var selectedRoles = model.RoleNames.Distinct();
                try
                {
                    await _userManager.AddToRolesAsync(user, selectedRoles);
                }
                catch (Exception ex)
                {
                    return new ServiceErrorResponse<bool>(ex.Message);
                }
            }
            else
            {
                try
                {
                    await _userManager.AddToRoleAsync(user, RoleName.Customer);
                }
                catch (Exception ex)
                {
                    return new ServiceErrorResponse<bool>(ex.Message);
                }
            }

            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> UpdateUserAsync(UserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString()) ??
                throw new ArgumentNullException($"Cannot find user by id");

            if (model.Email is not null)
            {
                if (!AppUtilities.IsValidEmail(model.Email))
                    return new ServiceErrorResponse<bool>("Cần nhập đúng định dạng email");

                if ((await _userManager.FindByEmailAsync(model.Email) != null) && user.Email != model.Email)
                {
                    return new ServiceErrorResponse<bool>("Email đã tồn tại");
                }
            }

            if ((await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber) != null) && user.PhoneNumber != model.PhoneNumber)
                return new ServiceErrorResponse<bool>("Số điện thoại đã được đăng ký");

            model.Password = model.ConfirmPassword = GenerateRandomPassword(length: 6);

            if (model.RoleNames != null && model.RoleNames.Any())
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);

                var selectedRoles = model.RoleNames.Distinct();
                try
                {
                    await _userManager.AddToRolesAsync(user, selectedRoles);
                }
                catch (Exception ex)
                {
                    return new ServiceErrorResponse<bool>(ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ServiceErrorResponse<bool>("Update user failed");
            }
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteUserByUserIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()); ;

            if (user == null) { throw new Exception($"Cannot find user: {id}"); }

            user.Deleted = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ServiceErrorResponse<bool>("Delete user failed");
            }
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<UserModel>> Get(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ??
                throw new ArgumentNullException("Cannot find user by that id");

            var roles = await _userManager.GetRolesAsync(user);

            var model = _mapper.Map<UserModel>(user);

            model.RoleNames = roles;

            var response = new ServiceResponse<UserModel>
            {
                Data = model,
                Success = true
            };
            return response;
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            var model = _mapper.Map<List<UserModel>>(await _context.Users.Where(x => x.Deleted == false).ToListAsync());
            return model;
        }

        public string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            StringBuilder password = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(validChars.Length);
                password.Append(validChars[randomIndex]);
            }

            return password.ToString();
        }

        public async Task<ServiceResponse<User>> GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Kiểm tra xem User.Identity.Name có null hay không
            if (httpContext == null || httpContext.User?.Identity?.Name == null)
            {
                return new ServiceErrorResponse<User>();
            }

            string username = httpContext.User.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            return new ServiceResponse<User>() { Success = user != null, Data = user };
        }

        public async Task<List<Role>> GetRolesByUserAsync(User user)
        {
            var list_role = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == user.Id
                            select r;
            return await list_role.ToListAsync();
        }

        public async Task<ServiceResponse<bool>> BanUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return new ServiceErrorResponse<bool>("User not found");

            user.IsBanned = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return new ServiceErrorResponse<bool>("Failed to ban user");

            return new ServiceSuccessResponse<bool>(true);
        }


    }
}
