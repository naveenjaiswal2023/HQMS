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
using SharedInfrastructure.ExternalServices;
using SharedInfrastructure.ExternalServices.Interfaces;
using SharedInfrastructure.Http;
using SharedInfrastructure.Settings;
using SharedInfrastructures.ExternalServices;

namespace QueueService.Infrastructure
{
    public static class DependencyInjection
    {
        {
            services.AddDbContext<QueueDbContext>(options =>

            services.AddSingleton<ServiceBusClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                if (string.IsNullOrWhiteSpace(connectionString))

                return new ServiceBusClient(connectionString);
            });

            // ✅ Azure Service Bus Publisher
            services.AddSingleton<IAzureServiceBusPublisher, AzureServiceBusPublisher>();

            // ✅ Domain Event Publisher (both MediatR + Azure Service Bus)
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

            // ✅ External APIs (binds POCO)
            services.Configure<ServiceApiOptions>(configuration.GetSection("Services"));

            //// ✅ HTTP Clients for External Services
            //services.AddHttpClient<IHospitalServiceClient, HospitalServiceClient>((sp, client) =>
            //{
            //    var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
            //    client.BaseAddress = new Uri(options.HospitalApi ?? throw new InvalidOperationException("HospitalApi is not configured."));
            //});

            //services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>((sp, client) =>
            //{
            //    var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
            //    client.BaseAddress = new Uri(options.AppointmentApi ?? throw new InvalidOperationException("AppointmentApi is not configured."));
            //});

            //services.AddHttpClient<IDoctorServiceClient, DoctorServiceClient>((sp, client) =>
            //{
            //    var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
            //    client.BaseAddress = new Uri(options.DoctorApi ?? throw new InvalidOperationException("DoctorApi is not configured."));
            //});

            //services.AddHttpClient<IPatientServiceClient, PatientServiceClient>((sp, client) =>
            //{
            //    var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
            //    client.BaseAddress = new Uri(options.PatientApi ?? throw new InvalidOperationException("PatientApi is not configured."));
            //});

            services.AddSingleton<IAzureServiceBusPublisher, AzureServiceBusPublisher>();

            // ✅ External API configuration
            services.Configure<ServiceApiOptions>(configuration.GetSection("Services"));

            services.AddHttpClient<IHospitalServiceClient, HospitalServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.HospitalApi ?? throw new InvalidOperationException("HospitalApi is not configured."));

            services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.AppointmentApi ?? throw new InvalidOperationException("AppointmentApi is not configured."));

            services.AddHttpClient<IDoctorServiceClient, DoctorServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.DoctorApi ?? throw new InvalidOperationException("DoctorApi is not configured."));

            services.AddHttpClient<IPatientServiceClient, PatientServiceClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceApiOptions>>().Value;
                client.BaseAddress = new Uri(options.PatientApi ?? throw new InvalidOperationException("PatientApi is not configured."));

            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IQueueItemRepository, QueueItemRepository>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
