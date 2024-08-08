using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using TCommerce.Services.TokenServices;
using TCommerce.Services.IRepositoryServices;
using System.Text.RegularExpressions;
using TCommerce.Data;
using TCommerce.Core.Models.Users;
using Microsoft.AspNetCore.Http;
using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Options;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Core.Utilities;
using TCommerce.Core.Models.Roles;
using TCommerce.Core.Models.SendMail;
using TCommerce.Core.Models.Accounts;
using TCommerce.Core.Models.Accounts.Account;
using TCommerce.Core.Models.Orders;
using TCommerce.Services.ProductServices;
using TCommerce.Services.ShoppingCartServices;
using TCommerce.Core.Interface;
using System.Net.Http;
using System.Data;
using TCommerce.Core.Models.Paging;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TCommerce.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailService;
        private readonly IOptions<UrlOptions> _urlOptions;
        private readonly SignInManager<User> _signInManager;
        private readonly IRepository<UserAddressMapping> _userAddressMappingRepository;
        private readonly IRepository<Address> _addressMappingRepository;
        private readonly IAddressService _addressService;
        private readonly IRepository<ShoppingCartItem> _shoppingCartRepository;
        private readonly IProductAttributeConverter _productAttributeConverter;
        public UserService(IMapper mapper, ApplicationDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor, IEmailSender emailService, IOptions<UrlOptions> urlOptions, SignInManager<User> signInManager, IRepository<UserAddressMapping> userAddressMappingRepository, IAddressService addressService, IRepository<Address> addressMappingRepository, IRepository<ShoppingCartItem> shoppingCartRepository, IProductAttributeConverter productAttributeConverter)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _urlOptions = urlOptions;
            _signInManager = signInManager;
            _userAddressMappingRepository = userAddressMappingRepository;
            _addressService = addressService;
            _addressMappingRepository = addressMappingRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _productAttributeConverter = productAttributeConverter;
        }

        public async Task<ServiceResponse<bool>> CreateUserAsync(User model, List<Guid>? roleIds = null, string? password = "")
        {
            var validationResult = await ValidateUserModelAsync(model);
            if (validationResult != null)
                return validationResult;

            var user = _mapper.Map<User>(model);
            user.CreatedDate = DateTime.Now;

            password ??= GenerateRandomPassword(length: 6);
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var addRoleResult = await AddRole(user, roleIds);
                if (!addRoleResult.Success)
                    return new ServiceErrorResponse<bool>(addRoleResult.Message);
            }

            return result.Succeeded
                ? new ServiceSuccessResponse<bool>()
                : new ServiceErrorResponse<bool>(FormatErrors(result.Errors));
        }
        public async Task<ServiceResponse<bool>> UpdateUserAsync(User model, List<Guid>? roleIds = null, string? password = "", bool requiredRandomPassword = false)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString())
                        ?? throw new ArgumentNullException($"Cannot find user by id");

            var validationResult = await ValidateUserModelAsync(model, user);
            if (validationResult != null)
                return validationResult;

            _mapper.Map(model, user);

            if (!string.IsNullOrEmpty(password) || requiredRandomPassword)
            {
                password ??= GenerateRandomPassword(length: 6);
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var addRoleResult = await AddRole(user, roleIds);
                if (!addRoleResult.Success)
                    return new ServiceErrorResponse<bool>(addRoleResult.Message);
            }

            return result.Succeeded
                ? new ServiceSuccessResponse<bool>()
                : new ServiceErrorResponse<bool>(FormatErrors(result.Errors));
        }
        private async Task<ServiceResponse<bool>> AddRole(User user, List<Guid>? roleIds)
        {
            if (roleIds is not null)
            {
                foreach (var roleId in roleIds)
                {
                    var role = await _roleManager.FindByIdAsync(roleId.ToString());
                    if (role != null && !await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        var result = await _userManager.AddToRoleAsync(user, role.Name);
                        if (!result.Succeeded)
                        {
                            return new ServiceErrorResponse<bool>(FormatErrors(result.Errors));
                        }
                    }
                }

            }
            var defaultRole = await _roleManager.FindByNameAsync(RoleName.Registerd);

            if (defaultRole != null && defaultRole.Name != null && !await _userManager.IsInRoleAsync(user, defaultRole.Name))
            {
                var result = (await _userManager.AddToRoleAsync(user, defaultRole.Name));
                if (!result.Succeeded)
                {
                    return new ServiceErrorResponse<bool>(FormatErrors(result.Errors));
                }
            }

            return new ServiceSuccessResponse<bool>();
        }
        private async Task<ServiceResponse<bool>> ValidateUserModelAsync(User model, User existingUser = null)
        {
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                if (!AppUtilities.IsValidEmail(model.Email))
                    return new ServiceErrorResponse<bool>("Cần nhập đúng định dạng email");

                if (existingUser?.Email != model.Email && await _userManager.FindByEmailAsync(model.Email) != null)
                    return new ServiceErrorResponse<bool>("Email đã tồn tại");
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                existingUser?.PhoneNumber != model.PhoneNumber &&
                await _context.Users.AnyAsync(x => x.PhoneNumber == model.PhoneNumber))
            {
                return new ServiceErrorResponse<bool>("Số điện thoại đã được đăng ký");
            }

            if (AppUtilities.IsValidEmail(model.UserName) && (existingUser is not null && model.UserName != existingUser.UserName))
            {
                return new ServiceErrorResponse<bool>("Username không nên là email");
            }

            return null;
        }

        private string FormatErrors(IEnumerable<IdentityError> errors)
        {
            return string.Join(", ", errors.Select(e => e.Description));
        }

        private bool ValidatePassword(string password, string confirmPassword)
        {
            return password is not null && confirmPassword is not null && password == confirmPassword;
        }

        private bool ValidateEmail(string email)
        {
            return email is not null && AppUtilities.IsValidEmail(email);
        }

        private async Task<bool> UserExistsByEmail(string email)
        {
            return email is not null && await _userManager.FindByEmailAsync(email) != null;
        }

        private async Task<bool> UserExistsByPhoneNumber(string phoneNumber)
        {
            return await _context.Users.AnyAsync(x => x.PhoneNumber == phoneNumber);
        }

        private bool IsValidUsername(string userName)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]*$");

            return userName is not null && regex.IsMatch(userName) && AppUtilities.IsValidEmail(userName);
        }
        public async Task<ServiceResponse<bool>> DeleteUserByUserIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null) { throw new Exception($"Cannot find user: {id}"); }

            if(await _userManager.IsInRoleAsync(user, RoleName.Admin))
            {
                return new ServiceErrorResponse<bool>("Cannot delete Admin");
            }

            user.Deleted = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ServiceErrorResponse<bool>("Delete user failed");
            }
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ??
                throw new ArgumentNullException("Cannot find user by that id");

            return user;
        }

        public async Task<PagedList<User>> GetAllUsersAsync(UserParameters userParameters)
        {
            IQueryable<User> usersQuery = _context.Users;

            // Lấy ID của người dùng dựa trên vai trò
            if (userParameters.Roles.Any())
            {
                var roleIds = await _context.UserRoles
                    .Where(ur => _context.Roles
                        .Where(r => userParameters.Roles.Contains(r.Name))
                        .Select(r => r.Id)
                        .Contains(ur.RoleId))
                    .Select(ur => ur.UserId)
                    .Distinct()
                    .ToListAsync();

                // Lọc người dùng dựa trên các ID
                usersQuery = usersQuery.Where(u => roleIds.Contains(u.Id));
            }

            if (userParameters.CreatedFromUtc.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.CreatedDate >= userParameters.CreatedFromUtc.Value);
            }

            if (userParameters.CreatedToUtc.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.CreatedDate <= userParameters.CreatedToUtc.Value);
            }

            if (!string.IsNullOrEmpty(userParameters.Email))
            {
                usersQuery = usersQuery.Where(u =>u.Email != null && u.Email.Contains(userParameters.Email));
            }

            if (!string.IsNullOrEmpty(userParameters.UserName))
            {
                usersQuery = usersQuery.Where(u => u.UserName != null && u.UserName.Contains(userParameters.UserName));
            }

            if (!string.IsNullOrEmpty(userParameters.FirstName))
            {
                usersQuery = usersQuery.Where(u => u.FirstName != null && u.FirstName.Contains(userParameters.FirstName));
            }

            if (!string.IsNullOrEmpty(userParameters.LastName))
            {
                usersQuery = usersQuery.Where(u => u.LastName != null && u.LastName.Contains(userParameters.LastName));
            }

            if (!string.IsNullOrEmpty(userParameters.Company))
            {
                usersQuery = usersQuery.Where(u => u.Company.Contains(userParameters.Company));
            }

            if (userParameters.DayOfBirth.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.Dob == userParameters.DayOfBirth.Value);
            }

            if (userParameters.Deleted)
            {
                usersQuery = usersQuery.Where(u => u.Deleted == userParameters.Deleted);
            }

            return await PagedList<User>.ToPagedList(usersQuery, userParameters.PageNumber, userParameters.PageSize);
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

        public async Task<User> GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is null");
            }

            var user = new User();

            if (httpContext.User.Identity.IsAuthenticated)
            {
                user = await _userManager.GetUserAsync(httpContext.User);
            }

            return user;
        }


        public async Task<List<Role>> GetRolesByUserAsync(User user)
        {
            var list_role = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == user.Id
                            select r;
            return await list_role.ToListAsync();
        }

        public async Task<ServiceResponse<bool>> BanUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return new ServiceErrorResponse<bool>("User not found");

            user.Deleted = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return new ServiceErrorResponse<bool>("Failed to ban user");

            return new ServiceSuccessResponse<bool>(true);
        }

        public async Task<bool> Logout(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString()) ??
                throw new ArgumentNullException("Cannot find user by id");

            user.RefreshToken = null;
            _context.Users.Update(user);
            _context.SaveChanges();
            return true;
        }

        //public async Task<ServiceResponse<string>> ConfirmEmail(string userId, string token)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user == null)
        //    {
        //        return new ServiceErrorResponse<string>($"Unable to load user with ID '{userId}'.");
        //    }

        //    string normalToken = DecodeToken(token);

        //    var result = await _userManager.ConfirmEmailAsync(user, normalToken);

        //    if (result.Succeeded)
        //        return new ServiceSuccessResponse<string>(_urlOptions.Value.ClientUrl);

        //    return new ServiceErrorResponse<string>("Email did not confirm");
        //}

        public async Task<ServiceResponse<string>> SendChangePasswordEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return new ServiceErrorResponse<string>("Tài khoản không tồn tại");

            var confirmEmailToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var validToken = EncodeToken(confirmEmailToken);

            string url = $"{_urlOptions.Value.ClientUrl}/account/reset-password?email={email}&token={validToken}";

            if (user.Email is not null)
            {
                EmailDto emailDto = new EmailDto
                {
                    Subject = "Đặt lại mật khẩu",
                    Body = $"<h1>Làm theo hướng dẫn để đặt lại mật khẩu của bạn</h1>" +
            $"<p>Tên đăng nhập của bạn là: </p><h3>{user.UserName}</h3>" +
            $"<p>Để đặt lại mật khẩu <a href='{url}'>Bấm vào đây</a></p>",
                    To = user.Email
                };
                await _emailService.SendEmailAsync(emailDto);
            }

            return new ServiceSuccessResponse<string>("Reset password URL has been sent to the email successfully!");
        }

        public async Task<ServiceResponse<string>> ResetPassword(ResetPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (!AppUtilities.IsValidEmail(model.Email))
                return new ServiceErrorResponse<string>("Cần nhập đúng định dạng email");

            if (user == null)
                return new ServiceErrorResponse<string>("No user associated with email");

            if (model.NewPassword != model.ConfirmPassword)
                return new ServiceErrorResponse<string>("Mật khẩu phải trùng khớp");

            user.RequirePasswordChange = false;
            await _context.SaveChangesAsync();

            string normalToken = DecodeToken(model.Token);

            try
            {
                var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

                if (result.Succeeded)
                    return new ServiceSuccessResponse<string>("Password has been reset successfully!");
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return new ServiceErrorResponse<string>(string.Join(", ", errors));
                }
            }
            catch
            {
                return new ServiceErrorResponse<string>("Something went wrong !");
            }
        }

        public string EncodeToken(string normalToken)
        {
            var encodedEmailToken = Encoding.UTF8.GetBytes(normalToken);
            return WebEncoders.Base64UrlEncode(encodedEmailToken);
        }
        public string DecodeToken(string encodeToken)
        {
            var decodedToken = WebEncoders.Base64UrlDecode(encodeToken);
            return Encoding.UTF8.GetString(decodedToken);
        }

        public async Task<ServiceResponse<bool>> UpdateUserAccountInfo(AccountInfoModel model)
        {
            var userModel = await GetCurrentUser();

            _mapper.Map(model, userModel);

            return await UpdateUserAsync(userModel);
        }

        public async Task<ServiceResponse<bool>> CreateUserAddressAsync(Address deliveryAddress)
        {
            var user = await GetCurrentUser();

            ArgumentNullException.ThrowIfNull(user);

            ArgumentNullException.ThrowIfNull(deliveryAddress);

            var province = await _addressService.GetProvinceByIdAsync(deliveryAddress.ProvinceId);

            var district = await _addressService.GetDistricteByIdAsync(deliveryAddress.DistrictId);

            var commune = await _addressService.GetCommuneByIdAsync(deliveryAddress.CommuneId);

            ArgumentNullException.ThrowIfNull(province);

            ArgumentNullException.ThrowIfNull(district);

            ArgumentNullException.ThrowIfNull(commune);

            var currentDefaultAddresses = await GetOwnAddressesAsync();

            if (deliveryAddress.IsDefault)
            {
                if (currentDefaultAddresses is not null)
                {
                    foreach (var item in currentDefaultAddresses)
                    {
                        var address = await _addressService.GetAddressByIdAsync(item.Id);

                        if (address is not null)
                        {
                            address.IsDefault = false;

                            await _addressService.UpdateAddressAsync(address);
                        }
                    }
                }
            }
            else
            {
                if (currentDefaultAddresses is null)
                {
                    deliveryAddress.IsDefault = true;
                }
            }

            if(deliveryAddress.Email is null)
            {
                deliveryAddress.Email = (await GetCurrentUser()).Email;
            }

            await _addressService.CreateAddressAsync(deliveryAddress);

            if (await _userAddressMappingRepository.Table
                    .FirstOrDefaultAsync(m => m.AddressId == deliveryAddress.Id && m.UserId == user.Id)
                is null)
            {
                var mapping = new UserAddressMapping
                {
                    AddressId = deliveryAddress.Id,
                    UserId = user.Id
                };

                await _userAddressMappingRepository.CreateAsync(mapping);
                return new ServiceSuccessResponse<bool>();
            }

            return new ServiceErrorResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteUserAddressAsync(int id)
        {
            var user = await GetCurrentUser();

            ArgumentNullException.ThrowIfNull(user);

            var address = await _addressService.GetAddressByIdAsync(id);

            if (address is not null)
            {
                if (await _userAddressMappingRepository.Table
                    .FirstOrDefaultAsync(m => m.AddressId == address.Id && m.UserId == user.Id)
                is UserAddressMapping mapping)
                {
                    await _userAddressMappingRepository.DeleteAsync(mapping.Id);
                    return new ServiceSuccessResponse<bool> { Message = "Success" };
                }
                await _addressService.DeleteAddressAsync(address.Id);
            }

            return new ServiceErrorResponse<bool>() { Message = "No address found" };
        }

        public async Task<List<AddressInfoModel>> GetOwnAddressesAsync()
        {
            var userId = (await GetCurrentUser()).Id;

            var query = from address in _addressMappingRepository.Table
                        join cam in _userAddressMappingRepository.Table on address.Id equals cam.AddressId
                        where cam.UserId == userId
                        select address;

            var addressList = await query.ToListAsync();

            var addressInfoList = new List<AddressInfoModel>();

            if (addressList is not null)
            {
                foreach (var item in addressList)
                {
                    var commune = (await _addressService.GetCommuneByIdAsync(item.CommuneId))?.Name;
                    var district = (await _addressService.GetDistricteByIdAsync(item.DistrictId))?.Name;
                    var province = (await _addressService.GetProvinceByIdAsync(item.ProvinceId))?.Name;

                    addressInfoList.Add(new AddressInfoModel()
                    {
                        Id = item.Id,
                        FullName = item.LastName + " " + item.FirstName,
                        AddressFull = $"{commune}, {district}, {province}",
                        PhoneNumber = item.PhoneNumber,
                        IsDefault = item.IsDefault
                    });
                }
            }

            return addressInfoList;
        }

        public async Task<ServiceResponse<bool>> UpdateUserAddressAsync(Address deliveryAddress)
        {
            var user = await GetCurrentUser();

            ArgumentNullException.ThrowIfNull(user);

            ArgumentNullException.ThrowIfNull(deliveryAddress);

            var province = await _addressService.GetProvinceByIdAsync(deliveryAddress.ProvinceId);

            var district = await _addressService.GetDistricteByIdAsync(deliveryAddress.DistrictId);

            var commune = await _addressService.GetCommuneByIdAsync(deliveryAddress.CommuneId);

            ArgumentNullException.ThrowIfNull(province);

            ArgumentNullException.ThrowIfNull(district);

            ArgumentNullException.ThrowIfNull(commune);

            if (deliveryAddress.IsDefault)
            {
                var currentDefaultAddresses = await GetOwnAddressesAsync();

                if (currentDefaultAddresses is not null)
                {
                    foreach (var item in currentDefaultAddresses)
                    {
                        if (item.Id != deliveryAddress.Id)
                        {
                            var address = await _addressService.GetAddressByIdAsync(item.Id);

                            if (address is not null)
                            {
                                address.IsDefault = false;

                                await _addressService.UpdateAddressAsync(address);
                            }
                        }
                    }
                }
            }

            if (deliveryAddress.Email is null)
            {
                deliveryAddress.Email = (await GetCurrentUser()).Email;
            }

            await _addressService.UpdateAddressAsync(deliveryAddress);

            return new ServiceSuccessResponse<bool>();
        }
    }
}

