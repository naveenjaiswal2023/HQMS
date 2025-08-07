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

            // ✅ Azure Service Bus
            services.AddSingleton<ServiceBusClient>(sp =>
            {
                var connectionString = configuration["ServiceBus:ConnectionString"]
                    ?? configuration.GetConnectionString("ServiceBus");

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Azure Service Bus connection string is not configured.");

                return new ServiceBusClient(connectionString);
            });

            services.AddSingleton<IAzureServiceBusPublisher, AzureServiceBusPublisher>();

            // ✅ Caching
            services.AddMemoryCache();

            // ✅ JWT Settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            // Register Internal Token Provider for internal service-to-service auth
            services.AddHttpClient<IInternalTokenProvider, InternalTokenProvider>();


            // Register HTTP handler for authenticated requests
            services.AddTransient<AuthenticatedHttpClientHandler>();
            services.AddHttpClient<IInternalTokenProvider, InternalTokenProvider>(client =>
            {
                var baseUrl = configuration["InternalApiBaseUrl"];
                if (string.IsNullOrWhiteSpace(baseUrl))
                    throw new InvalidOperationException("Missing 'InternalApiBaseUrl' in configuration.");

                client.BaseAddress = new Uri(baseUrl);
            });

            // ✅ Domain Services
            services.AddScoped<IAuthDbContext, AuthDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
            services.AddScoped<IAuthService, Application.Services.AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IEmailSender, EmailSender>();

            // ✅ Accessor
            services.AddHttpContextAccessor();

            return services;
        }
    }
}