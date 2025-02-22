using Azure.Storage.Queues;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.Text;
using SendGrid.Helpers.Mail;

namespace API.Services
{
    public class SendNotificationsQueueClient : ISendNotificationsQueueClient
    {
        private readonly string _connectionString;
        private readonly string _userQueueName;
        private readonly string _companyQueueName;
        private readonly string _newApplicantCompanyQueueName;
        private readonly string _newApplicantPredictionQueueName;
        private readonly string _updatedApplicationStatusQueueName;

        public SendNotificationsQueueClient(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("AzureBlobStorage:ConnectionString").Value;
            _userQueueName = configuration.GetSection("UserNotificationsQueueName").Value;
            _companyQueueName = configuration.GetSection("CompanyNotificationsQueueName").Value;
            _newApplicantCompanyQueueName = configuration.GetSection("CompanyNewApplicantNotificationsQueueName").Value;
            _newApplicantPredictionQueueName = configuration.GetSection("CompanyNewApplicantPredictionQueueName").Value;
            _updatedApplicationStatusQueueName = configuration.GetSection("UpdatedApplicationStatusQueueName").Value;
        }

        public async Task SendMessageToUserAsync(JobPostNotificationQueueMessage jobPostNotificationMessage)
        {
            // Create a QueueClient
            QueueClient queueClient = new QueueClient(_connectionString, _userQueueName);

            // Ensure the queue exists (create if it doesn't)
            await queueClient.CreateIfNotExistsAsync();

            if (queueClient.Exists())
            {
                string messageContent = JsonConvert.SerializeObject(jobPostNotificationMessage);

                // Encode the message content as Base64
                byte[] messageBytes = Encoding.UTF8.GetBytes(messageContent);
                string encodedMessage = Convert.ToBase64String(messageBytes);

                // Send the Base64 encoded message to the queue
                await queueClient.SendMessageAsync(encodedMessage);

                Console.WriteLine($"Message sent to queue: {messageContent}");
            }
            else
            {
                Console.WriteLine("Queue does not exist.");
            }
        }

        public async Task SendMessageToCompanyAsync(JobPostNotificationQueueMessage jobPostNotificationMessage)
        {
            // Create a QueueClient
            QueueClient queueClient = new QueueClient(_connectionString, _companyQueueName);

            // Ensure the queue exists (create if it doesn't)
            await queueClient.CreateIfNotExistsAsync();

            if (queueClient.Exists())
            {
                string messageContent = JsonConvert.SerializeObject(jobPostNotificationMessage);

                // Encode the message content as Base64
                byte[] messageBytes = Encoding.UTF8.GetBytes(messageContent);
                string encodedMessage = Convert.ToBase64String(messageBytes);

                // Send the Base64 encoded message to the queue
                await queueClient.SendMessageAsync(encodedMessage);

                Console.WriteLine($"Message sent to queue: {messageContent}");
            }
            else
            {
                Console.WriteLine("Queue does not exist.");
            }
        }

        public async Task SendNewApplicantMessageToCompanyAsync(NewApplicantQueueMessage message)
        {
            // Create a QueueClient
            QueueClient queueClient = new QueueClient(_connectionString, _newApplicantCompanyQueueName);

            // Ensure the queue exists (create if it doesn't)
            await queueClient.CreateIfNotExistsAsync();

            if (queueClient.Exists())
            {
                string messageContent = JsonConvert.SerializeObject(message);

                // Encode the message content as Base64
                byte[] messageBytes = Encoding.UTF8.GetBytes(messageContent);
                string encodedMessage = Convert.ToBase64String(messageBytes);

                // Send the Base64 encoded message to the queue
                await queueClient.SendMessageAsync(encodedMessage);

                Console.WriteLine($"Message sent to queue: {messageContent}");
            }
            else
            {
                Console.WriteLine("Queue does not exist.");
            }
        }

        public async Task SendNewApplicantPredictionMessageAsync(NewApplicantPredictionQueueMessage message)
        {
            // Create a QueueClient
            QueueClient queueClient = new QueueClient(_connectionString, _newApplicantPredictionQueueName);
            await SendMessage(queueClient, message);
        }

        public async Task SendMessageToUserOnUpdateApplicationStatusAsync(ApplicantStatusUpdated message)
        {
            // Create a QueueClient
            QueueClient queueClient = new QueueClient(_connectionString, _newApplicantPredictionQueueName);
            await SendMessage(queueClient, message);
        }

        private async Task SendMessage(QueueClient queueClient, object message)
        {
            // Ensure the queue exists (create if it doesn't)
            await queueClient.CreateIfNotExistsAsync();

            if (queueClient.Exists())
            {
                string messageContent = JsonConvert.SerializeObject(message);

                // Encode the message content as Base64
                byte[] messageBytes = Encoding.UTF8.GetBytes(messageContent);
                string encodedMessage = Convert.ToBase64String(messageBytes);

                // Send the Base64 encoded message to the queue
                await queueClient.SendMessageAsync(encodedMessage);

                Console.WriteLine($"Message sent to queue: {messageContent}");
            }
            else
            {
                Console.WriteLine("Queue does not exist.");
            }
        }
    }
}
