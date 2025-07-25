using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QueueService.Application.Common.Interfaces;
using QueueService.Application.Handlers.Commands;
using QueueService.Application.Interfaces;
using QueueService.Domain.Interfaces;
using QueueService.Domain.Interfaces.ExternalServices;
using QueueService.Infrastructure.Events;
using QueueService.Infrastructure.Messaging;
using QueueService.Infrastructure.Persistence;
using QueueService.Infrastructure.Repositories;
using QueueService.Infrastructure.Services;
using QueueService.Infrastructure.Services.ExternalServices;
using SharedInfrastructure.Settings;

namespace QueueService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ✅ EF Core: SQL Server
            services.AddDbContext<QueueDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // ✅ Azure Service Bus client (singleton)
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
            services.Configure<ServiceApiOptions>(configuration.GetSection("Services"));

            // ✅ HTTP Clients for external services
            services.AddHttpClient<IHospitalServiceClient, HospitalServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.HospitalApi ?? throw new InvalidOperationException("HospitalApi is not configured."));
            });

            services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.AppointmentApi ?? throw new InvalidOperationException("AppointmentApi is not configured."));
            });

            services.AddHttpClient<IDoctorServiceClient, DoctorServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.DoctorApi ?? throw new InvalidOperationException("DoctorApi is not configured."));
            });

            services.AddHttpClient<IPatientServiceClient, PatientServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.PatientApi ?? throw new InvalidOperationException("PatientApi is not configured."));
            });

            // ✅ Cache & Context
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            // ✅ Scoped application services
            //services.AddMediatR(typeof(CreateQueueItemCommandHandler).Assembly);
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IQueueItemRepository, QueueItemRepository>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }
    }
}
