using T.Library.Model.Response;
using T.Library.Model;
using T.Library.Model.Users;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using Microsoft.AspNetCore.Identity;
using T.Library.Model.ViewsModel;
using App.Utilities;

namespace T.WebApi.Services.UserServices
{
    public interface IUserService
    {
        Task<List<UserModel>> GetAllAsync();
        Task<List<Role>> GetAllRolesAsync();
        Task<ServiceResponse<UserModel>> Get(Guid id);
        Task<ServiceResponse<bool>> CreateOrEditAsync(UserModel model);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserService(IMapper mapper, DatabaseContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ServiceResponse<bool>> CreateOrEditAsync(UserModel model)
        {
            using (_context)
            {
                if (!AppUtilities.IsValidEmail(model.Email))
                    return new ServiceErrorResponse<bool>("Cần nhập đúng định dạng email");

                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    return new ServiceErrorResponse<bool>("Email đã tồn tại");
                }

                if (await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber) != null)
                    return new ServiceErrorResponse<bool>("Số điện thoại đã được đăng ký");

                var userTable = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (userTable == null)
                {
                    var user = new User();
                    user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.PhoneNumber = model.PhoneNumber;
                    user.UserName = model.UserName;
                    user.CreatedDate = AppExtensions.GetDateTimeNow();
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (model.RoleNames != null && model.RoleNames.Any())
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, userRoles);

                        var selectedRoles = model.RoleNames.Distinct();
                        try
                        {
                            await _userManager.AddToRolesAsync(user, selectedRoles);
                        }
                        catch(Exception ex)
                        {
                            return new ServiceErrorResponse<bool>(ex.Message);
                        }
                    }
                }
                else
                {
                    var userMapped = _mapper.Map<User>(model);

                    if (_context.IsRecordUnchanged(userTable, userMapped))
                    {
                        return new ServiceErrorResponse<bool>("Data is unchanged");
                    }

                    await _userManager.FindByIdAsync(model.Id.ToString());
                    //userTable.Dob = model.Dob;
                    userTable.Email = model.Email;
                    userTable.FirstName = model.FirstName;
                    userTable.LastName = model.LastName;
                    userTable.PhoneNumber = model.PhoneNumber;
                    userTable.UserName = model.UserName;

                    if (model.RoleNames != null && model.RoleNames.Any())
                    {
                        var userRoles = await _userManager.GetRolesAsync(userTable);
                        await _userManager.RemoveFromRolesAsync(userTable, userRoles);

                        var selectedRoles = model.RoleNames.Distinct();
                        try
                        {
                            await _userManager.AddToRolesAsync(userTable, selectedRoles);
                        }
                        catch (Exception ex)
                        {
                            return new ServiceErrorResponse<bool>(ex.Message);
                        }
                    }
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        userTable.PasswordHash = _userManager.PasswordHasher.HashPassword(userTable, model.Password);

                    }
                    var result = await _userManager.UpdateAsync(userTable);
                    if (!result.Succeeded)
                    {
                        return new ServiceErrorResponse<bool>("Update user failed");
                    }
                }
                
                return new ServiceSuccessResponse<bool>();
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()); ;

            if (user == null || user.Deleted == true) { throw new Exception($"Cannot find user: {id}"); }
            user.Deleted = true;

            await _userManager.UpdateAsync(user);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ServiceErrorResponse<bool>("Delete user failed");
            }
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<UserModel>> Get(Guid id)
        {
            using (_context)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

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
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            using (_context)
            {
                var model = _mapper.Map<List<UserModel>>(await _context.Users.ToListAsync());
                return model;
            }
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            using (_context)
            {
                return await _roleManager.Roles.ToListAsync();
            }
        }
    }
}
