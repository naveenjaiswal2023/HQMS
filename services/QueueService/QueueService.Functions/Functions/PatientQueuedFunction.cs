using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using QueueService.Domain.Events;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueueService.Functions.Functions;

public class PatientQueuedFunction
{
    private readonly ILogger _logger;

    public PatientQueuedFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PatientQueuedFunction>();
    }

    [Function("PatientQueuedFunction")]
    public async Task RunAsync(
    [ServiceBusTrigger("queue-event.topic", "queue-event.sub", Connection = "ServiceBus:ConnectionString")] string message)
    {
        try
        {
            _logger.LogInformation("Received raw message: {Message}", message);

            var @event = JsonSerializer.Deserialize<PatientQueuedEvent>(message, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (@event == null)
            {
                _logger.LogWarning("Deserialized PatientQueuedEvent is null.");
            }
            else
            {
                _logger.LogInformation("Deserialized PatientQueuedEvent with QueueNumber: {QueueNumber}", @event.QueueNumber);
            }


            // TODO: Save snapshot or notify doctor dashboard
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "JSON deserialization error for message: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error processing PatientQueuedEvent.");
        }
    }

}