using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Enum;
using T.Library.Model.Users;
using T.WebApi.Extensions;

namespace T.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> roleManager;
        public SeedController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Seed()
        {
            var rolenames = typeof(RoleName).GetFields();
            foreach (var item in rolenames)
            {
                string? name = item.GetRawConstantValue().ToString();
                var ffound = await roleManager.FindByNameAsync(name);
                if (ffound == null)
                {
                    await roleManager.CreateAsync(new Role(name));
                }
            }

            var user2 = await _userManager.FindByEmailAsync("hovanthanh12102002@gmail.com");
            if (user2 == null)
            {
                user2 = new User()
                {
                    Id = Guid.NewGuid(),

                    FirstName = "Văn Thành",
                    LastName = "Hồ",
                    Email = "hovanthanh12102002@gmail.com",
                    NormalizedEmail = "hovanthanh12102002@gmail.com",
                    PhoneNumber = "032232131",
                    UserName = "thanhhv",
                    NormalizedUserName = "THANHHV",
                    CreatedDate = AppExtensions.GetDateTimeNow(),
                    EmailConfirmed = true // không cần xác thực email nữa , 
                };
                await _userManager.CreateAsync(user2, "123321");
                await _userManager.AddToRoleAsync(user2, RoleName.Customer);
                return Ok();
            }
            return BadRequest();
        }
    }
}
