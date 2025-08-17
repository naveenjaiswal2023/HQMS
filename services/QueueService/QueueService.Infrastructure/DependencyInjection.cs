using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QueueService.Application.Common.Interfaces;
using QueueService.Application.Handlers.Commands;
using QueueService.Application.Services;
using QueueService.Domain.Interfaces;
using QueueService.Infrastructure.Events;
using QueueService.Infrastructure.Messaging;
using QueueService.Infrastructure.Persistence;
using QueueService.Infrastructure.Repositories;
using QueueService.Infrastructure.Services;
using SharedInfrastructure.ExternalServices;
using SharedInfrastructure.ExternalServices.Interfaces;
using SharedInfrastructure.Http;
using SharedInfrastructure.Settings;
using SharedInfrastructures.ExternalServices;

namespace QueueService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration,
            string actualConnectionString)
        {
            // ✅ Register DbContext using provided connection string
            services.AddDbContext<QueueDbContext>(options =>
                options.UseSqlServer(actualConnectionString));

            services.AddDbContextFactory<QueueDbContext>(options =>
                options.UseSqlServer(actualConnectionString),
                ServiceLifetime.Scoped); // Ensures scoped lifetime matching IUnitOfWork

            // ✅ Azure Service Bus Client setup
            services.AddSingleton<ServiceBusClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString =
                    config["ServiceBus:ConnectionString"] ??
                    config["AzureServiceBus:ConnectionString"];

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Azure Service Bus connection string is missing.");

                return new ServiceBusClient(connectionString);
            });

            // ✅ Azure Service Bus Publisher
            services.AddSingleton<IAzureServiceBusPublisher, AzureServiceBusPublisher>();

            // ✅ Domain Event Publisher (both MediatR + Azure Service Bus)
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

            // ✅ External APIs (binds POCO)
            services.Configure<ServiceApiOptions>(configuration.GetSection("Services"));

            services.AddSingleton<IAzureServiceBusPublisher, AzureServiceBusPublisher>();

            // ✅ External API configuration
            services.Configure<ServiceApiOptions>(configuration.GetSection("Services"));

            // ✅ Register IInternalTokenProvider with HttpClient support
            services.AddHttpClient<IInternalTokenProvider, InternalTokenProvider>();

            // ✅ DelegatingHandler that uses the token provider
            services.AddTransient<AuthenticatedHttpClientHandler>();

            // ✅ Register HttpClients for each service and attach the auth handler
            services.AddHttpClient<IHospitalServiceClient, HospitalServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.HospitalApi ?? throw new InvalidOperationException("HospitalApi is not configured."));
            }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.AppointmentApi ?? throw new InvalidOperationException("AppointmentApi is not configured."));
            }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddHttpClient<IDoctorServiceClient, DoctorServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.DoctorApi ?? throw new InvalidOperationException("DoctorApi is not configured."));
            }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddHttpClient<IPatientServiceClient, PatientServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.PatientApi ?? throw new InvalidOperationException("PatientApi is not configured."));
            }).AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            // ✅ Common Infrastructure
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();

            // ✅ MediatR: CQRS handlers
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(CreateQueueItemCommandHandler).Assembly));

            // ✅ Application-layer abstractions
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IQueueQueryService, QueueQueryService>();
            services.AddScoped<IQueueItemRepository, QueueItemRepository>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
