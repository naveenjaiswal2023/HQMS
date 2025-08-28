using AuthService.Functions.Middleware;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults(worker =>
            {
                // ✅ Global middleware for centralized exception handling/logging
                worker.UseMiddleware<ExceptionHandlingMiddleware>();
            })
            .ConfigureServices(services =>
            {
                // ✅ Logging
                services.AddLogging();

                // ✅ Register application services
                //services.AddScoped<IEmailService, EmailService>(); // Your email implementation

                // ✅ Any other domain/application service registrations
                // services.AddScoped<IUserService, UserService>();
                // services.AddScoped<IQueueService, QueueService>();
            })
            .Build();

        host.Run();
    }
}
