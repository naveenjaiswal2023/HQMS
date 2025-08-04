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
using SharedInfrastructure.ExternalServices;
using SharedInfrastructure.ExternalServices.Interfaces;
using SharedInfrastructure.Http;
using SharedInfrastructure.Settings;
using SharedInfrastructures.ExternalServices;

namespace AppointmentService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, string actualConnectionString)
        {
            // ✅ EF Core: SQL Server with actual connection string
            services.AddDbContext<AppointmentDbContext>(options =>
                options.UseSqlServer(actualConnectionString));

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
            services.Configure<ServiceApiOptions>(configuration.GetSection("ServicesAuth"));

            // ✅ Register token provider used by AuthenticatedHttpClientHandler
            services.AddScoped<IInternalTokenProvider, InternalTokenProvider>();

            // ✅ Register Authenticated handler
            services.AddTransient<AuthenticatedHttpClientHandler>();

            // ✅ Register HttpClients with message handler
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

            // ✅ Cache & Context
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            // ✅ Scoped application services
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }

}
