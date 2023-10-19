using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.WebApi.Extensions;

namespace T.WebApi.Services.DataSeederService
{
    public class UserRoleMappingDataSeed : SingletonBase<UserRoleMappingDataSeed>
    {
        public List<UserRoleMappingModel> GetAll()
        {
            return new List<UserRoleMappingModel>()
            {
                //new UserRoleMappingModel()
                //{
                //    Users = new List<User>()
                //    {
                //        new User()
                //        {
                //            Id = Guid.NewGuid(),
                //            FirstName = "Văn Thành",
                //            LastName = "Hồ",
                //            Email = "hovanthanh12102002@gmail.com",
                //            NormalizedEmail = "hovanthanh12102002@gmail.com",
                //            PhoneNumber = "0322321312",
                //            UserName = "thanhhv",
                //            NormalizedUserName = "THANHHV",
                //            CreatedDate = DateTime.UtcNow,
                //            EmailConfirmed = true
                //        }, 
                        
                //    },
                //    Roles = new List<Role>()
                //    {
                //        new Role(RoleName.Admin)
                //    }
                //},
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
                            CreatedDate = AppExtensions.GetDateTimeNow(),
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
