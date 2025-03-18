using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Devices.Application.Devices.Commands.RegisterDevice;
using Backbone.Modules.Devices.Application.Devices.Queries.ListDevices;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Devices.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration applicationConfiguration)
    {
        services.ConfigureAndValidate<ApplicationConfiguration>(applicationConfiguration.Bind);

        applicationConfiguration.GetSection("IdentityDeletion").Bind(IdentityDeletionConfiguration.Instance);

        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<RegisterDeviceCommand>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(Validator).Assembly);
        services.AddScoped<ChallengeValidator>();

        AddEventHandlers(services);
    }

    private static void AddEventHandlers(IServiceCollection services)
    {
        foreach (var eventHandler in GetAllDomainEventHandlers())
        {
            services.AddTransient(eventHandler);
        }
    }

    private static IEnumerable<Type> GetAllDomainEventHandlers()
    {
        var domainEventHandlerTypes =
            from t in Assembly.GetExecutingAssembly().GetTypes()
            from i in t.GetInterfaces()
            where t.IsClass && !t.IsAbstract && i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
            select t;

        return domainEventHandlerTypes;
    }
}
