using AuthService.Domain.Events;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace AuthService.Functions.Functions
{
    public class UserRegisteredFunction
    {
        private readonly ILogger _logger;
        // private readonly IEmailService _emailService;

        public UserRegisteredFunction(ILoggerFactory loggerFactory /*, IEmailService emailService */)
        {
            _logger = loggerFactory.CreateLogger<UserRegisteredFunction>();
            // _emailService = emailService;
        }

        [Function("UserRegisteredFunction")]
        public async Task Run(
            [ServiceBusTrigger("user.registered.topic", "user.registered.sub", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message)
        {
            try
            {
                var body = message.Body.ToString();
                var userEvent = JsonConvert.DeserializeObject<UserRegisteredEvent>(body);

                if (userEvent == null || string.IsNullOrWhiteSpace(userEvent.Email))
                {
                    _logger.LogWarning("Invalid UserRegisteredEvent payload. MessageId: {MessageId}, Body: {Body}",
                        message.MessageId, body);
                    return; // don’t throw → avoid DLQ
                }

                _logger.LogInformation("New user registered: {Email}, MessageId: {MessageId}",
                    userEvent.Email, message.MessageId);

                var subject = "Welcome to HQMS";
                // var bodyContent = $"<p>Hello {userEvent.FullName},</p><p>Welcome to our platform! Your account has been created successfully.</p>";
                // await _emailService.SendEmailAsync(userEvent.Email, subject, bodyContent);

                _logger.LogInformation("Welcome email sent to {Email}", userEvent.Email);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON Deserialization failed. MessageId: {MessageId}", message.MessageId);
                // ❌ Don’t rethrow → otherwise Service Bus retries DLQ
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while processing messageId {MessageId}", message.MessageId);
                throw; // retry transient errors → may go DLQ if persistent
            }
        }
    }
}
