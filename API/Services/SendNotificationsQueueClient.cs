using Azure.Storage.Queues;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.Text;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    public class SendNotificationsQueueClient : ISendNotificationsQueueClient
    {
        private readonly string _connectionString;
        private readonly string _newApplicantPredictionQueueName;
        private readonly string _notificationsDelegatedServiceName;
        private readonly ILogger<SendNotificationsQueueClient> _logger;

        public SendNotificationsQueueClient(IConfiguration configuration, ILogger<SendNotificationsQueueClient> logger)
        {
            _connectionString = configuration.GetSection("BlobStorage:ConnectionString").Value;
            _notificationsDelegatedServiceName = configuration.GetSection("NotificationsDelegatedServiceName").Value;
            _newApplicantPredictionQueueName = configuration.GetSection("CompanyNewApplicantPredictionQueueName").Value;
            _logger = logger;
        }

        public async Task SendMessageToUserAsync(NotificationEventMessage message)
        {
            // Create a QueueClient
            QueueClient queueClient = new QueueClient(_connectionString, _notificationsDelegatedServiceName);
            await SendMessage(queueClient, message);
        }

        public async Task SendMessageToCompanyAsync(NotificationEventMessage message)
        {
            QueueClient queueClient = new QueueClient(_connectionString, _notificationsDelegatedServiceName);
            await SendMessage(queueClient, message);
        }

        public async Task SendNewApplicantMessageToCompanyAsync(NotificationEventMessage message)
        {
            QueueClient queueClient = new QueueClient(_connectionString, _notificationsDelegatedServiceName);
            await SendMessage(queueClient, message);
        }

        public async Task SendNewApplicantPredictionMessageAsync(NewApplicantPredictionQueueMessage message)
        {
            try
            {
                QueueClient queueClient = new QueueClient(_connectionString, _newApplicantPredictionQueueName);
                await SendMessage(queueClient, message);
            }
            catch(Exception ex)
            {
                _logger.LogError($"There was an issue while trying to send event for prediction: {ex.Message}");
                throw;//WE MUST THROW THERE, BECAUSE THIS OPERATION IS CRITICAL, AND IF EVENT IS NOT SENT WE SHOULD NOT TAKE USER CREDITS
            }
        }

        public async Task SendMessageToUserOnUpdateApplicationStatusAsync(NotificationEventMessage message)
        {
            QueueClient queueClient = new QueueClient(_connectionString, _notificationsDelegatedServiceName);
            await SendMessage(queueClient, message);
        }

        private async Task SendMessage(QueueClient queueClient, object message)
        {
            await queueClient.CreateIfNotExistsAsync();

            if (queueClient.Exists())
            {
                string messageContent = JsonConvert.SerializeObject(message);

                _logger.LogInformation("Serialized message: {MessageContent}", messageContent);

                byte[] messageBytes = Encoding.UTF8.GetBytes(messageContent);
                string encodedMessage = Convert.ToBase64String(messageBytes);

                _logger.LogDebug("Encoded message in Base64: {EncodedMessage}", encodedMessage);

                await queueClient.SendMessageAsync(encodedMessage);

                _logger.LogInformation("Message sent to queue: {MessageContent}", messageContent);
            }
            else
            {
                _logger.LogError("Queue does not exist.");
            }
        }

    }
}
