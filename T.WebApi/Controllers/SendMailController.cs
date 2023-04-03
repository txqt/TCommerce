using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> SendMail(string email, string subject, string message)
        {
            await _emailSender.SendEmailAsync(email, subject, message);
            return NoContent();
        }
    }
}
