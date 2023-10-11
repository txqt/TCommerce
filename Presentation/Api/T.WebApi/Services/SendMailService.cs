using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using T.Library.Model.SendMail;

public class MailSettings
{
    public string? Mail { get; set; }
    public string? DisplayName { get; set; }
    public string? Password { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }

}

public interface IEmailSender
{
    Task SendEmailAsync(EmailDto emailDto);
    Task SendSmsAsync(string number, string message);
}

public class SendMailService : IEmailSender
{


    private readonly MailSettings mailSettings;

    private readonly ILogger<SendMailService> logger;


    // mailSetting được Inject qua dịch vụ hệ thống
    // Có inject Logger để xuất log
    public SendMailService(IOptions<MailSettings> _mailSettings, ILogger<SendMailService> _logger)
    {
        mailSettings = _mailSettings.Value;
        logger = _logger;
        logger.LogInformation("Create SendMailService");
    }


    public async Task SendEmailAsync(EmailDto emailDto)
    {
        var message = new MimeMessage();
        message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
        message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
        message.To.Add(MailboxAddress.Parse(emailDto.To));
        message.Subject = emailDto.Subject;


        var builder = new BodyBuilder();
        builder.HtmlBody = emailDto.Body;
        message.Body = builder.ToMessageBody();

        // dùng SmtpClient của MailKit
        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            //smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            //smtp.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13 | SslProtocols.None;
            //smtp.CheckCertificateRevocation = false;
            smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.Auto);
            smtp.Authenticate(mailSettings.Mail, "lvlk rcah icbz kcun");
            await smtp.SendAsync(message);
        }

        catch (Exception ex)
        {
            // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
            System.IO.Directory.CreateDirectory("mailssave");
            var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
            await message.WriteToAsync(emailsavefile);

            logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
            logger.LogError(ex.Message);
        }

        smtp.Disconnect(true);

        logger.LogInformation("send mail to " + emailDto.To);


    }

    public Task SendSmsAsync(string number, string message)
    {
        // Cài đặt dịch vụ gửi SMS tại đây
        Directory.CreateDirectory("smssave");
        var emailsavefile = string.Format(@"smssave/{0}-{1}.txt", number, Guid.NewGuid());
        File.WriteAllTextAsync(emailsavefile, message);
        return Task.FromResult(0);
    }
}