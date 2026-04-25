using MailKit.Net.Smtp;
using MimeKit;

namespace RecyclonicApi.HelperServices
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Recyclonic Backend", _config["EmailSettings:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"],
                                    int.Parse(_config["EmailSettings:Port"]),
                                    MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(_config["EmailSettings:Username"],
                                         _config["EmailSettings:Password"]);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
