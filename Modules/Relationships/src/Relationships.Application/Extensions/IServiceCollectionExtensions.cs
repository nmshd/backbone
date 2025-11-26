using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Relationships.Application.Extensions;

public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            services.AddMediatR(c => c
                .RegisterServicesFromAssemblyContaining<CreateRelationshipTemplateCommand>()
                .AddOpenBehavior(typeof(LoggingBehavior<,>))
                .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
                .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
            );

            services.AddValidatorsFromAssembly(typeof(Validator).Assembly);

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
            where t.IsClass && t is { IsAbstract: false, IsGenericType: true } && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
            select t;

        return domainEventHandlerTypes;
    }
}
