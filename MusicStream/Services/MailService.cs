using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MusicStream.Models;
using System;
using System.Threading.Tasks;

namespace MusicStream.Services
{
    public class MailService : ISendMailService
    {
        private readonly MailSettings mailSettings;

        // mailSetting được Inject qua dịch vụ hệ thống
        // Có inject Logger để xuất log
        public MailService(IOptions<MailSettings> _mailSettings)
        {
            mailSettings = _mailSettings.Value;
        }

        // Gửi email, theo nội dung trong mailContent
        public async Task<bool> SendEmail(MailRequest mailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailContent.ToEmail));
            email.Subject = mailContent.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Body;
            email.Body = builder.ToMessageBody();

            // dùng SmtpClient của MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception)
            {
                return false;
            }

            smtp.Disconnect(true);

            return true;
        }
    }
}
