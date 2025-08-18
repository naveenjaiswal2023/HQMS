using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using QueueService.Application.Handlers.Commands;
using QueueService.Application.Queries;
using QueueService.Functions.Middleware;
using QueueService.Functions.Publishers;
using QueueService.Infrastructure;
using SharedInfrastructure;
using SharedInfrastructure.DTO;
using SharedInfrastructure.ExternalServices;
using SharedInfrastructure.ExternalServices.Interfaces;
using SharedInfrastructure.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

var host = new HostBuilder()
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        // Global Exception Middleware
        worker.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Validate and load connection strings early
        var queueDbConnectionString = GetRequiredEnvironmentVariable("QueueDbConnectionString");
        var serviceBusConnectionString = GetRequiredEnvironmentVariable("QueueServiceBusConnectionString");
        var appointmentApi = GetRequiredConfigurationValue(configuration, "Services:AppointmentApi");

        Console.WriteLine($"[Function Startup] Configuration loaded successfully");

        // ✅ Infrastructure Services
        services.AddInfrastructureServices(configuration, queueDbConnectionString);

        // ✅ AutoMapper
        services.AddAutoMapper(typeof(CreateQueueItemCommandHandler).Assembly);

        // ✅ MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetUpcomingAppointmentsQuery).Assembly);
        });

        // ✅ Configure Service Authentication
        services.Configure<ClientCredentialDto>(configuration.GetSection("ServiceAuth"));
        services.AddScoped<IInternalTokenProvider, InternalTokenProvider>();
        services.AddTransient<AuthenticatedHttpClientHandler>();

        // ✅ Azure Service Bus Client
        services.AddSingleton<ServiceBusClient>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<ServiceBusClient>>();
            logger.LogInformation("Initializing Service Bus Client");

            return new ServiceBusClient(serviceBusConnectionString, new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets,
                RetryOptions = new ServiceBusRetryOptions
                {
                    MaxRetries = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    MaxDelay = TimeSpan.FromSeconds(30),
                    Mode = ServiceBusRetryMode.Exponential
                }
            });
        });

        // ✅ Register Polly retry policy (as IAsyncPolicy)
        services.AddSingleton<IAsyncPolicy>(sp =>
        {
            var log = sp.GetRequiredService<ILogger<NotificationEventPublisher>>();
            return Policy.Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        log.LogWarning(exception, "Retry {RetryCount} after {Delay}s", retryCount, timespan.TotalSeconds);
                    });
        });

        // ✅ Register Polly circuit breaker policy (as IAsyncPolicy)
        services.AddSingleton<IAsyncPolicy>(sp =>
        {
            var log = sp.GetRequiredService<ILogger<NotificationEventPublisher>>();
            return Policy.Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (ex, breakDelay) =>
                    {
                        log.LogWarning("⚡ Circuit opened for {BreakDelay}s due to {Message}", breakDelay.TotalSeconds, ex.Message);
                    },
                    onReset: () =>
                    {
                        log.LogInformation("✅ Circuit closed, operations normal");
                    });
        });

        // ✅ NotificationEventPublisher via factory:
        // pick the right policies by concrete type (no order dependency, no custom interfaces)
        services.AddScoped<NotificationEventPublisher>(sp =>
        {
            var client = sp.GetRequiredService<ServiceBusClient>();
            var logger = sp.GetRequiredService<ILogger<NotificationEventPublisher>>();
            var policies = sp.GetServices<IAsyncPolicy>().ToList();

            var retry = policies.OfType<AsyncRetryPolicy>().FirstOrDefault()
                       ?? throw new InvalidOperationException("Retry policy not registered.");
            var circuit = policies.OfType<AsyncCircuitBreakerPolicy>().FirstOrDefault()
                         ?? throw new InvalidOperationException("Circuit breaker policy not registered.");

            return new NotificationEventPublisher(client, retry, circuit, logger);
        });

        // ✅ HTTP Client with retry policy
        services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>(client =>
        {
            client.BaseAddress = new Uri(appointmentApi);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
        .AddPolicyHandler(GetHttpRetryPolicy());

        // ✅ Logging configuration
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddSingleton<IConfiguration>(configuration);
    })
    .Build();

host.Run();


// ===== Helpers =====
static string GetRequiredEnvironmentVariable(string variableName)
{
    var value = Environment.GetEnvironmentVariable(variableName);
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"Environment variable '{variableName}' is missing or empty.");
    }
    return value;
}

static string GetRequiredConfigurationValue(IConfiguration configuration, string key)
{
    var value = configuration[key];
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"Configuration value '{key}' is missing or empty.");
    }
    return value;
}

static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                if (outcome.Exception != null)
                {
                    Console.WriteLine($"HTTP Retry {retryCount} after {timespan.TotalSeconds}s due to: {outcome.Exception.Message}");
                }
                else
                {
                    Console.WriteLine($"HTTP Retry {retryCount} after {timespan.TotalSeconds}s due to status: {outcome.Result?.StatusCode}");
                }
            });
}
