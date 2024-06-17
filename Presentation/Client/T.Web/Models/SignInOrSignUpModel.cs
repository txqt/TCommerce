using T.Library.Model;

namespace T.Web.Models
{
    public class SignInOrSignUpModel
    {
        public SignInOrSignUpModel()
        {
            RegisterRequest = new RegisterRequest();
            AccessTokenRequest = new AccessTokenRequestModel();
        }

        public RegisterRequest RegisterRequest { get; set; }
        public AccessTokenRequestModel AccessTokenRequest { get; set; }
    }
}
