using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.SendMail;
using T.WebApi.Attribute;

namespace T.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
