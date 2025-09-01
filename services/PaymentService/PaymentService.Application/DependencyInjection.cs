using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Common.Behaviours;
using System.Reflection;

namespace PaymentService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // Remove or comment out the following line if you are not using FluentValidation.AspNetCore >= 11
            // services.AddFluentValidationAutoValidation(); // Optional, if using Web API

            // MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

            return services;
        }
    }
}