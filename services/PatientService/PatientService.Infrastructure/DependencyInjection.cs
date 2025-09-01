using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientService.Application.Common.Interfaces;
using PatientService.Domain.Interfaces;
using PatientService.Infrastructure.Events;
using PatientService.Infrastructure.Persistence;
using PatientService.Infrastructure.Repositories;
using PatientService.Infrastructure.Services;
using Polly;
using Polly.Extensions.Http;
using SharedInfrastructure.ExternalServices;
using SharedInfrastructure.ExternalServices.Interfaces;
using SharedInfrastructure.Http;
using SharedInfrastructure.Settings;
using System.Reflection;

namespace PatientService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration,
            string actualConnectionString)
        {
            // ✅ DbContext (scoped by default)
            services.AddDbContext<PatientDbContext>(options =>
                options.UseSqlServer(actualConnectionString));

            // ✅ Azure Service Bus Client
            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString =
                    config["ServiceBus:ConnectionString"] ??
                    config["AzureServiceBus:ConnectionString"];

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Azure Service Bus connection string is missing.");

                return new ServiceBusClient(connectionString);
            });

            // ✅ Domain Event Publisher
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

            // ✅ External APIs (config bind)
            services.Configure<ServiceApiOptions>(configuration.GetSection("Services"));

            // ✅ HttpClient + Auth Handler
            services.AddHttpClient<IInternalTokenProvider, InternalTokenProvider>();
            services.AddTransient<AuthenticatedHttpClientHandler>();

            // ✅ Polly Policies
            var retryPolicy = GetRetryPolicy();
            var circuitBreakerPolicy = GetCircuitBreakerPolicy();

            // ✅ Register HttpClients for external services
            //RegisterExternalHttpClients(services, retryPolicy, circuitBreakerPolicy);

            // ✅ Common Infrastructure
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();

            // ✅ MediatR for Application Layer
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.Load("PatientService.Application")));

            // ✅ Application-layer abstractions
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPatientRepository, PatientRepository>();

            services.AddHealthChecks();

            return services;
        }

        //private static void RegisterExternalHttpClients(IServiceCollection services,
        //    IAsyncPolicy<HttpResponseMessage> retryPolicy,
        //    IAsyncPolicy<HttpResponseMessage> circuitBreakerPolicy)
        //{
            //services.AddHttpClient<IHospitalServiceClient, HospitalServiceClient>((sp, client) =>
            //{
            //    var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
            //    client.BaseAddress = new Uri(options.HospitalApi ?? throw new InvalidOperationException("HospitalApi is not configured."));
            //})
            //.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
            //.AddPolicyHandler(retryPolicy)
            //.AddPolicyHandler(circuitBreakerPolicy);

            //services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>((sp, client) =>
            //{
            //    var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
            //    client.BaseAddress = new Uri(options.AppointmentApi ?? throw new InvalidOperationException("AppointmentApi is not configured."));
            //})
            //.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
            //.AddPolicyHandler(retryPolicy)
            //.AddPolicyHandler(circuitBreakerPolicy);

            //services.AddHttpClient<IDoctorServiceClient, DoctorServiceClient>((sp, client) =>
            //{
            //    var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
            //    client.BaseAddress = new Uri(options.DoctorApi ?? throw new InvalidOperationException("DoctorApi is not configured."));
            //})
            //.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
            //.AddPolicyHandler(retryPolicy)
            //.AddPolicyHandler(circuitBreakerPolicy);

            //services.AddHttpClient<IPatientServiceClient, PatientServiceClient>((sp, client) =>
            //{
            //    var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
            //    client.BaseAddress = new Uri(options.PatientApi ?? throw new InvalidOperationException("PatientApi is not configured."));
            //})
            //.AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
            //.AddPolicyHandler(retryPolicy)
            //.AddPolicyHandler(circuitBreakerPolicy);
        //}

        // ----------------------------
        // 🔹 Polly Resilience Policies
        // ----------------------------
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"[Retry] Attempt {retryAttempt}, waiting {timespan.TotalSeconds}s. Reason: {outcome.Exception?.Message}");
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, timespan) =>
                    {
                        Console.WriteLine($"[CircuitBreaker] Opened for {timespan.TotalSeconds}s due to: {outcome.Exception?.Message}");
                    },
                    onReset: () => Console.WriteLine("[CircuitBreaker] Closed. Requests flowing normally."),
                    onHalfOpen: () => Console.WriteLine("[CircuitBreaker] Half-open. Testing requests...")
                );
        }
    }
}