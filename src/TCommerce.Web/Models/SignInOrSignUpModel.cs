using TCommerce.Core.Models.Accounts;

namespace TCommerce.Web.Models
{
    public class LoginModel
    {
        public LoginModel()
        {
            RegisterRequest = new RegisterRequest();
            AccessTokenRequest = new AccessTokenRequestModel();
        }

        public RegisterRequest RegisterRequest { get; set; }
        public AccessTokenRequestModel AccessTokenRequest { get; set; }
    }
}
