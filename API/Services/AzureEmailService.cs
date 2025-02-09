using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public class AzureEmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly IConfiguration _configuration;
        private readonly string _emailConnString;
        private readonly string _senderEmail;

        public AzureEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _emailConnString = _configuration.GetSection("AzureEmailConnectionString").Value;
            _senderEmail = _configuration.GetSection("SenderEmail").Value;
            _emailClient = new EmailClient(_emailConnString);
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                var emailMessage = new EmailMessage(_senderEmail, recipientEmail, new EmailContent(subject) { PlainText = body });
                var response = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        public async Task SendEmailWithTemplateAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                var emailMessage = new EmailMessage(_senderEmail, recipientEmail, new EmailContent(subject) { Html = body });
                var response = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}
