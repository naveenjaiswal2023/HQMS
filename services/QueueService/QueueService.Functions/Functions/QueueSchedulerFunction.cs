using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QueueService.Application.Commands;
using QueueService.Application.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

public class QueueSchedulerFunction
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueueSchedulerFunction> _logger;

    public QueueSchedulerFunction(IServiceProvider serviceProvider, ILogger<QueueSchedulerFunction> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    [Function("QueueSchedulerFunction")]
    public async Task RunAsync(
        [TimerTrigger("0 */5 * * * *")] TimerInfo timer, // Every 5 minutes
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[QueueSchedulerFunction] Started at {DateTime.UtcNow}");

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            // Use a query to fetch upcoming appointments via MediatR (CQRS style)
            var upcomingAppointments = await mediator.Send(new GetUpcomingAppointmentsQuery(15), cancellationToken);

            foreach (var appt in upcomingAppointments)
            {
                try
                {
                    // Change: Convert appt.QueueNumber from string to int when passing to CreateQueueItemCommand
                    var command = new CreateQueueItemCommand(
                        appt.DoctorId,
                        appt.PatientId,
                        appt.AppointmentId,
                        appt.DepartmentId,
                        appt.HospitalId,
                        appt.QueueNumber
                    );

                    await mediator.Send(command, cancellationToken);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to schedule queue for appointment {appt.AppointmentId}");
                }
            }

            _logger.LogInformation($"[QueueSchedulerFunction] Completed at {DateTime.UtcNow}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[QueueSchedulerFunction] Error executing scheduled task");
        }
    }
}
