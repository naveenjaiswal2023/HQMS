using AuthService.Application.Common.Interfaces;
using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Events;
using AuthService.Infrastructure.Messaging;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedInfrastructure.ExternalServices;
using SharedInfrastructure.ExternalServices.Interfaces;
using SharedInfrastructure.Http;
using SharedInfrastructure.Settings;

namespace AuthService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ✅ Database Context
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<ServiceBusClient>(sp =>
            {
                return new ServiceBusClient(connectionString);
            });

            services.AddSingleton<IAzureServiceBusPublisher, AzureServiceBusPublisher>();

            // ✅ Caching
            services.AddMemoryCache();


            //services.AddHttpClient<IDoctorServiceClient, DoctorServiceClient>(client =>
            //{
            //    client.BaseAddress = new Uri(configuration["ServiceUrls:StaffService"]);
            //});


                client.BaseAddress = new Uri(baseUrl);
            });

            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICacheService, CacheService>();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<IAuthService, AuthService.Application.Services.AuthService>();
            services.AddScoped<IEmailSender, EmailSender>();
            //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GenerateDoctorQueueCommand).Assembly));

            // ✅ Accessor
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
