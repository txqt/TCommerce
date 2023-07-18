using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace T.WebApi.Extensions
{
    public class CustomUserManager<TUser> : UserManager<TUser> where TUser : class
    {
        public CustomUserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer,
                  errors, services, logger)
        {
        }

        public override async Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            try
            {
                return await base.CreateAsync(user, password);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException && (sqlException.Number == 2601 || sqlException.Number == 2627))
                {
                    var duplicateField = GetDuplicateField(sqlException.Message);
                    var errors = new List<IdentityError>
                {
                    new IdentityError { Code = $"Duplicate{duplicateField}", Description = $"{duplicateField} đã tồn tại." }
                };
                    return IdentityResult.Failed(errors.ToArray());
                }
                else
                {
                    throw; // Ném lại exception gốc nếu không phải lỗi trùng lặp
                }
            }
        }

        private string GetDuplicateField(string errorMessage)
        {
            // Phân tích chuỗi thông báo lỗi để xác định trường bị trùng lặp
            // Ví dụ: "Cannot insert duplicate key row in object 'dbo.Users' with unique index 'IX_Users_Email'. The duplicate key value is (hovanthanh12102002@gmail.com).\r\nThe statement has been terminated."
            // Trả về "Email" là trường bị trùng lặp

            // Đoạn mã sau chỉ là một ví dụ đơn giản, bạn có thể tùy chỉnh hàm này phù hợp với cấu trúc thông báo lỗi của hệ thống cơ sở dữ liệu của bạn
            var startIndex = errorMessage.IndexOf("'IX_") + 4;
            var endIndex = errorMessage.IndexOf("'", startIndex);
            var field = errorMessage.Substring(startIndex, endIndex - startIndex);
            return field;
        }
    }

}
