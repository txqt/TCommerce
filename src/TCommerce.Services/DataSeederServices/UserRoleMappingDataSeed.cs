using TCommerce.Core.Extensions;
using TCommerce.Core.Models.Roles;
using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Users;

namespace TCommerce.Services.DataSeederServices
{
    public class UserRoleMappingDataSeed : SingletonBase<UserRoleMappingDataSeed>
    {
        public List<UserRoleMappingModel> GetAll()
        {
            return new List<UserRoleMappingModel>()
            {
                new UserRoleMappingModel()
                {
                    Users = new List<User>()
                    {
                        new User()
                        {
                            Id = Guid.NewGuid(),
                            FirstName = "Văn Thành",
                            LastName = "Hồ",
                            Email = "hovanthanh@gmail.com",
                            NormalizedEmail = "hovanthanh@gmail.com",
                            PhoneNumber = "0322321311",
                            UserName = "thanhhv2",
                            NormalizedUserName = "THANHHV2",
                            CreatedDate = DateTime.Now,
                            EmailConfirmed = true // không cần xác thực email nữa , 
                        }
                    },
                    Roles = new List<Role>()
                    {
                        new Role(RoleName.Customer),
                        new Role(RoleName.Employee)
                    }
                }
            };
        }
    }
}
