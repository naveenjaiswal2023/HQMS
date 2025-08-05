using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueueService.Application.Handlers.Commands;
using QueueService.Application.Queries;
using QueueService.Functions.Middleware;
using QueueService.Infrastructure;
using SharedInfrastructure;
using SharedInfrastructure.DTO;
using SharedInfrastructure.ExternalServices;
using SharedInfrastructure.ExternalServices.Interfaces;
using SharedInfrastructure.Http;

public class Program
{
    public static void Main(string[] args)
    {
        var host = new HostBuilder()
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables(); // Enables reading from actual system env variables
    })
            .ConfigureFunctionsWorkerDefaults(worker =>
            {
                
                worker.UseMiddleware<ExceptionHandlingMiddleware>();
            })
    .ConfigureServices((context, services) =>
            {

                services.AddMediatR(cfg =>
                {
                });

            })
            .Build();

        host.Run();
    }
}
