using T.Library.Model.Response;
using T.Library.Model;
using T.Library.Model.Users;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using Microsoft.AspNetCore.Identity;
using T.Library.Model.ViewsModel;

namespace T.WebApi.Services.UserServices
{
    public interface IUserService
    {
        Task<List<UserModel>> GetAllAsync();
        Task<ServiceResponse<User>> Get(int id);
        Task<ServiceResponse<bool>> CreateOrEditAsync(UserModel model);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserService(IMapper mapper, DatabaseContext context, UserManager<User> userManager)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
        }

        public async Task<ServiceResponse<bool>> CreateOrEditAsync(UserModel model)
        {
            using (_context)
            {
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
                    foreach (var roleName in model.Roles)
                    {
                        await _userManager.AddToRoleAsync(user, roleName);
                    }
                }
                else
                {
                    var userMapped = _mapper.Map<User>(model);
                    if (_context.IsRecordUnchanged(userTable, userMapped))
                    {
                        return new ServiceErrorResponse<bool>("Data is unchanged");
                    }

                    if (await _userManager.Users.AnyAsync(x => x.Email == model.Email))
                    {
                        return new ServiceErrorResponse<bool>("Email đã tồn tại");
                    }

                    await _userManager.FindByIdAsync(model.Id.ToString());
                    //userTable.Dob = model.Dob;
                    userTable.Email = model.Email;
                    userTable.FirstName = model.FirstName;
                    userTable.LastName = model.LastName;
                    userTable.PhoneNumber = model.PhoneNumber;
                    userTable.UserName = model.UserName;
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        userTable.PasswordHash = _userManager.PasswordHasher.HashPassword(userTable, model.Password);

                    }
                    var result = await _userManager.UpdateAsync(userTable);
                    if (!result.Succeeded)
                    {
                        return new ServiceErrorResponse<bool>("Create user failed");
                    }
                }
                
                return new ServiceSuccessResponse<bool>();
            }
        }

        public Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<User>> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            using (_context)
            {
                var model = _mapper.Map<List<UserModel>>(await _context.Users.ToListAsync());
                return model;
            }
        }
    }
}
