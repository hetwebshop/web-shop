using Azure;
using Azure.Communication.Email;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace jobify_azure_function
{
    public class EmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly IConfiguration _configuration;
        private readonly string _emailConnString;
        private readonly string _senderEmail;
        private readonly string _blobConnString;
        private readonly string _blobContainer;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _emailConnString = _configuration.GetSection("AzureEmailConnectionString").Value;
            _senderEmail = _configuration.GetSection("SenderEmail").Value;
            _emailClient = new EmailClient(_emailConnString);
            _blobConnString = configuration.GetSection("AzureWebJobsStorage").Value;
            _blobContainer = configuration.GetSection("BlobContainerName").Value;
            _logger = logger;
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

        public async Task SendEmailWithTemplateAsync(string recipientEmail, string subject, string body, string cvFilePath = null, string cvFileName = null)
        {
            try
            {
                var emailMessage = new EmailMessage(_senderEmail, recipientEmail, new EmailContent(subject) { Html = body });

                if (!string.IsNullOrEmpty(cvFilePath))
                {
                    var blobClient = new BlobServiceClient(_blobConnString);
                    var containerClient = blobClient.GetBlobContainerClient(_blobContainer);
                    var blobClientForCv = containerClient.GetBlobClient(Path.GetFileName(cvFilePath));

                    var downloadInfo = await blobClientForCv.DownloadAsync();

                    using (var memoryStream = new MemoryStream())
                    {
                        await downloadInfo.Value.Content.CopyToAsync(memoryStream);

                        var attachment = new EmailAttachment(
                            cvFileName,
                            "application/pdf", 
                            new BinaryData(memoryStream.ToArray())
                        );

                        emailMessage.Attachments.Add(attachment);
                    }
                }

                var response = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Došlo je do greške prilikom slanja maila. Provjerite email providera ili da li je email ispravan: {recipientEmail}");
            }
        }
    }
}
