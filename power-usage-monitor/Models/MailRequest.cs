using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;


namespace power_usage_monitor.Models
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html, string from = null);
    }
    public class MailRequest : IEmailService
    {
        private readonly IConfiguration? _config;

        public MailRequest(IConfiguration config)
        {
            _config = config;
        }

        public void Send(string to, string subject, string html, string from = null)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _config.GetSection("MailSettings:MailAddress").Value));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("MailSettings:Host").Value, 
                Convert.ToInt32(_config.GetSection("MailSettings:Port").Value), SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("MailSettings:DisplayName").Value,
                _config.GetSection("MailSettings:Password").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}




