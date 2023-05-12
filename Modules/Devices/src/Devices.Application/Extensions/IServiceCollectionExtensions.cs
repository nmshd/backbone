using System.Reflection;
using Backbone.Modules.Devices.Application.AutoMapper;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backbone.Modules.Devices.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<RegisterDeviceCommand>());
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(RegisterDeviceCommandValidator).Assembly);
        services.AddScoped<ChallengeValidator>();
        AddEventHandlers(services);
    }

    private static void AddEventHandlers(IServiceCollection services)
    {
        foreach (var eventHandler in GetAllIntegrationEventHandlers())
        {
            services.AddTransient(eventHandler);
        }
    }

    private static IEnumerable<Type> GetAllIntegrationEventHandlers()
    {
        var integrationEventHandlerTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
                                           from i in t.GetInterfaces()
                                           where t.IsClass && !t.IsAbstract && i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)
                                           select t;

        return integrationEventHandlerTypes;
    }
}
