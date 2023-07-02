using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Enum;
using T.Library.Model.SendMail;
using T.WebApi.Attribute;

namespace T.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class SendMailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        public SendMailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> SendMail([FromForm]EmailDto emailDto)
        {
            await _emailSender.SendEmailAsync(emailDto);
            return NoContent();
        }
    }
}
