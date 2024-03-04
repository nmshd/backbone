using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Relationships.Application.AutoMapper;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Relationships.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<CreateRelationshipTemplateCommand>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );

        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateRelationshipTemplateCommandValidator).Assembly);
    }
}
