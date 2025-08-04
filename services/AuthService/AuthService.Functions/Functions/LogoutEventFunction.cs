using AuthService.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace AuthService.Functions.Functions
{
    public class LogoutEventFunction
    {
        private readonly ILogger<LogoutEventFunction> _logger;

        public LogoutEventFunction(ILogger<LogoutEventFunction> logger)
        {
            _logger = logger;
        }

        [Function("LogoutEventFunction")]
        public void Run(
            [ServiceBusTrigger("user.loggedout.topic", "user.loggedout.sub", Connection = "AzureServiceBusConnectionString")]
            string message,
            FunctionContext context)
        {
            _logger.LogInformation("Received logout event message from Service Bus.");

            try
            {
                var logoutEvent = JsonSerializer.Deserialize<LogoutEventDto>(message);

                if (logoutEvent != null)
                {
                    _logger.LogInformation($"Logout Event: UserId = {logoutEvent.UserId}, " +
                                           $"LogoutTime = {logoutEvent.LoggedOutAt}");
                }
                else
                {
                    _logger.LogWarning("Deserialized LogoutEventDto is null.");
                }
            }
            catch (JsonException jex)
            {
                _logger.LogError(jex, "JSON deserialization failed for logout event message.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing logout event message.");
            }
        }
    }
}
