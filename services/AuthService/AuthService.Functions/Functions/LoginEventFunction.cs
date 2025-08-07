using AuthService.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace AuthService.Functions.Functions
{
    public class LoginEventFunction
    {
        private readonly ILogger<LoginEventFunction> _logger;

        public LoginEventFunction(ILogger<LoginEventFunction> logger)
        {
            _logger = logger;
        }

        [Function("LoginEventFunction")]
        public void Run(
            [ServiceBusTrigger("user.loggedin.topic", "user.loggedin.sub", Connection = "ServiceBusConnection")]
            string message,
            FunctionContext context)
        {
            _logger.LogInformation("Received login event message from Service Bus.");

            try
            {
                var loginEvent = JsonSerializer.Deserialize<LoginEventDto>(message);

                if (loginEvent != null)
                {
                    _logger.LogInformation($"Login Event: UserId = {loginEvent.UserId}, " +
                                           $"LoginTime = {loginEvent.LoginTime}, " +
                                           $"IP = {loginEvent.IpAddress}");
                }
                else
                {
                    _logger.LogWarning("Deserialized LoginEventDto is null.");
                }
            }
            catch (JsonException jex)
            {
                _logger.LogError(jex, "JSON deserialization failed for login event message.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing login event message.");
            }
        }
    }
}
