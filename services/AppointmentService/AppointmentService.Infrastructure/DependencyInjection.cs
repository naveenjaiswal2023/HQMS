using AppointmentService.Application.Common.Interfaces;
using AppointmentService.Domain.Interfaces;
using AppointmentService.Infrastructure.Events;
using AppointmentService.Infrastructure.Messaging;
using AppointmentService.Infrastructure.Persistence;
using AppointmentService.Infrastructure.Repositories;
using AppointmentService.Infrastructure.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using SharedInfrastructure.ExternalServices;
using SharedInfrastructure.ExternalServices.Interfaces;
using SharedInfrastructure.Http;
using SharedInfrastructure.Settings;
using System.Net.Http;

namespace AppointmentService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, string actualConnectionString)
        {
            services.AddDbContext<AppointmentDbContext>(options =>
            options.UseSqlServer(actualConnectionString, sqlOptions =>
            {
                // 🔹 Enable built-in retry on transient errors
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,                // Retry up to 5 times
                    maxRetryDelay: TimeSpan.FromSeconds(10), // Wait up to 10s between retries
                    errorNumbersToAdd: null          // You can specify SQL error codes if needed
                );
            }));


            // ✅ Azure Service Bus
            services.AddSingleton<ServiceBusClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config["ServiceBus:ConnectionString"];
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Azure Service Bus connection string is missing in configuration.");

                return new ServiceBusClient(connectionString);
            });

            services.AddSingleton<IAzureServiceBusPublisher, AzureServiceBusPublisher>();

            // ✅ External API configuration
            services.Configure<ServiceApiOptions>(configuration.GetSection("ServicesApi"));

            // ✅ Token provider & handler
            services.AddScoped<IInternalTokenProvider, InternalTokenProvider>();
            services.AddTransient<AuthenticatedHttpClientHandler>();

            // 🔹 Define Retry & CircuitBreaker Policies
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound) // Optional
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Exponential backoff

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );

            // 🔹 Register HttpClients with Polly policies
            services.AddHttpClient<IHospitalServiceClient, HospitalServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.HospitalApi ?? throw new InvalidOperationException("HospitalApi is not configured."));
            })
            .AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

            services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.AppointmentApi ?? throw new InvalidOperationException("AppointmentApi is not configured."));
            })
            .AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

            services.AddHttpClient<IDoctorServiceClient, DoctorServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.DoctorApi ?? throw new InvalidOperationException("DoctorApi is not configured."));
            })
            .AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

            services.AddHttpClient<IPatientServiceClient, PatientServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.PatientApi ?? throw new InvalidOperationException("PatientApi is not configured."));
            })
            .AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

            // Cache & Context
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            // Scoped application services
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();

            return services;
        }
    }
}
