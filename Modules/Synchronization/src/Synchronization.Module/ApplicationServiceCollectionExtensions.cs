using PushDatawalletModifications = Backbone.Modules.Synchronization.Module.Features.Datawallets.PushDatawalletModifications;
using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Synchronization.Module;

internal static class ApplicationServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            services.AddMediatR(c => c
                .RegisterServicesFromAssemblyContaining<PushDatawalletModifications.Command>()
                .AddOpenBehavior(typeof(LoggingBehavior<,>))
                .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
                .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
            );

            services.AddValidatorsFromAssembly(typeof(PushDatawalletModifications.Command).Assembly);
            services.AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            foreach (var eventHandler in GetAllDomainEventHandlers())
            {
                services.AddTransient(eventHandler);
            }
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
