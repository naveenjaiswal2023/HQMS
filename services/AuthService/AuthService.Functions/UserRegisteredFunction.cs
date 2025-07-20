using AuthService.Domain.Events;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AuthService.Domain.Interfaces;

namespace AuthService.Functions
{
    public class UserRegisteredFunction
    {
        private readonly ILogger _logger;
        //private readonly IEmailService _emailService;

        public UserRegisteredFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UserRegisteredFunction>();
            //_emailService = emailService;
        }

        [Function("UserRegisteredFunction")]
        public async Task Run(
            [ServiceBusTrigger("user.registered.topic", "user.registered.sub", Connection = "ServiceBusConnection")]
            string message)
        {
            var userEvent = JsonConvert.DeserializeObject<UserRegisteredEvent>(message);
            _logger.LogInformation($"New user registered: {userEvent.Email}");

            var subject = "Welcome to HQMS";
            //var body = $"<p>Hello {userEvent.FullName},</p><p>Welcome to our platform! Your account has been created successfully.</p>";

            //await _emailService.SendEmailAsync(userEvent.Email, subject, body);

            _logger.LogInformation("Welcome email sent.");
        }
    }
}
