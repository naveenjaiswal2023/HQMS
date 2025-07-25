using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueueService.Application.Queries;
using QueueService.Functions.Middleware;

public class Program
{
    public static void Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults(worker =>
            {
                
                worker.UseMiddleware<ExceptionHandlingMiddleware>();
            })
            .ConfigureServices(services =>
            {
                // ✅ Add logging
                services.AddLogging();

                // ✅ Register MediatR handlers from Application layer
                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(GetUpcomingAppointmentsQuery).Assembly); // Replace with a real IRequest or handler
                });

                // ✅ Register application services, repositories, etc.
                // services.AddScoped<IUserService, UserService>();
                // services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            })
            .Build();

        host.Run();
    }
}
