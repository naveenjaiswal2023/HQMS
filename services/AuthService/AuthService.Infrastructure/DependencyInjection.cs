using Azure.Messaging.ServiceBus;
using HQMS.QueueService.Application.Commands;
using HQMS.QueueService.Application.Common.Interfaces;
using HQMS.QueueService.Application.Handlers.Events;
using HQMS.QueueService.Domain.Interfaces;
using HQMS.QueueService.Infrastructure.Persistence;
using HQMS.QueueService.Infrastructure.Repositories;
using HQMS.QueueService.Infrastructure.Services;
using HQMS.QueueService.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HQMS.QueueService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });

            // Azure Service Bus
            services.AddSingleton<ServiceBusClient>(serviceProvider =>
            {
                var connectionString = configuration.GetConnectionString("ServiceBus");
                return new ServiceBusClient(connectionString);
            });

            //services.AddHttpClient<IPatientServiceClient, PatientServiceClient>(client =>
            //{
            //    client.BaseAddress = new Uri(configuration["ServiceUrls:PatientService"]);
            //});

            //services.AddHttpClient<IDoctorServiceClient, DoctorServiceClient>(client =>
            //{
            //    client.BaseAddress = new Uri(configuration["ServiceUrls:StaffService"]);
            //});

            // Repositories
            services.AddScoped<IQueueItemRepository, QueueItemRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor(); // Required!
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddSingleton<ServiceBusClient>(sp =>
            new ServiceBusClient(configuration["AzureServiceBus:ConnectionString"]));
            services.AddHostedService<QueueScheduler>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GenerateDoctorQueueCommand).Assembly));
            services.AddScoped<IQueueDomainService, QueueDomainService>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            return services;
        }
    }
}
