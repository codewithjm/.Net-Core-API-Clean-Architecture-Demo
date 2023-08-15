using Business.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;

namespace Business;

public static class ServiceRegistration
{
    public static void AddBusiness(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly()); 
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLoggingBehavior<,>));
        services.AddSingleton(Log.Logger);
    }
}