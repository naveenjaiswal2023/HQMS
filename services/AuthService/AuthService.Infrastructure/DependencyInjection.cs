using AuthService.Application.Common.Interfaces;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Events;
using AuthService.Infrastructure.Messaging;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedInfrastructure.Settings;

namespace AuthService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Redis Cache
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = configuration.GetConnectionString("Redis");
            //});

            // Azure Service Bus
            services.AddSingleton<ServiceBusClient>(serviceProvider =>
            {
                var connectionString = configuration.GetConnectionString("ServiceBus");
                return new ServiceBusClient(connectionString);
            });

            services.AddSingleton<ServiceBusClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config["ServiceBus:ConnectionString"];
                return new ServiceBusClient(connectionString);
            });

            services.AddSingleton<IAzureServiceBusPublisher, AzureServiceBusPublisher>();



            //services.AddHttpClient<IPatientServiceClient, PatientServiceClient>(client =>
            //{
            //    client.BaseAddress = new Uri(configuration["ServiceUrls:PatientService"]);
            //});

            //services.AddHttpClient<IDoctorServiceClient, DoctorServiceClient>(client =>
            //{
            //    client.BaseAddress = new Uri(configuration["ServiceUrls:StaffService"]);
            //});

            // Repositories


            //// Services
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

            services.AddMemoryCache();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICacheService, CacheService>();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<IAuthService, AuthService.Application.Services.AuthService>();
            services.AddScoped<IEmailSender, EmailSender>();
            //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GenerateDoctorQueueCommand).Assembly));


            return services;
        }
    }
}
