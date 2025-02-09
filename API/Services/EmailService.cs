using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace API.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _sendGridApiKey;
        private readonly string _senderEmail;

        public EmailService(IConfiguration configuration)
        {
            _sendGridApiKey = configuration["EmailSettings:SendGridApiKey"];
            _senderEmail = configuration["EmailSettings:SenderEmail"];
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_senderEmail, "HET");
            var to = new EmailAddress(recipientEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

            var response = await client.SendEmailAsync(msg);
        }

        public async Task SendEmailWithTemplateAsync(string recipientEmail, string subject, string body)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_senderEmail, "HET");
            var to = new EmailAddress(recipientEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, htmlContent: body);

            var response = await client.SendEmailAsync(msg);
        }
    }
}
