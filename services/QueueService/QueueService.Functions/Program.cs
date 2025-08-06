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
        var configuration = context.Configuration;

        // ✅ Get actual connection string from ENV
        var QueueDbConnectionString = Environment.GetEnvironmentVariable("QueueDbConnectionString");
        //var QueueServiceBusConnectionString = Environment.GetEnvironmentVariable("QueueServiceBusConnectionString");

        if (string.IsNullOrWhiteSpace(QueueDbConnectionString))
            throw new InvalidOperationException("QueueDbConnectionString environment variable is missing.");

        Console.WriteLine($"[Function Startup] Connection String Loaded: {QueueDbConnectionString}");

        // ✅ Register Infrastructure with real connection string
        services.AddInfrastructureServices(configuration, QueueDbConnectionString);

        // ✅ AutoMapper registration
        services.AddAutoMapper(typeof(CreateQueueItemCommandHandler).Assembly);

        // ✅ MediatR registration
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetUpcomingAppointmentsQuery).Assembly);
        });

        services.AddTransient<AuthenticatedHttpClientHandler>();
        services.Configure<ClientCredentialDto>(configuration.GetSection("ServiceAuth"));
        services.AddScoped<IInternalTokenProvider, InternalTokenProvider>();


        // ✅ External service client (with token)
        var appointmentApi = configuration["Services:AppointmentApi"];
        if (string.IsNullOrWhiteSpace(appointmentApi))
            throw new InvalidOperationException("AppointmentApi is not configured in settings.");

        services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>(client =>
        {
            client.BaseAddress = new Uri(appointmentApi);
        })
        .AddHttpMessageHandler<AuthenticatedHttpClientHandler>(); // 👈 attach token handler


        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
    })
    .Build();

host.Run();