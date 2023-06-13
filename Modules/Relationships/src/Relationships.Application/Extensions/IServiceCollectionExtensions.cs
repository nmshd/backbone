using Backbone.Modules.Relationships.Application.AutoMapper;
using Backbone.Modules.Relationships.Application.MediatR;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;
using Enmeshed.BuildingBlocks.Application.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backbone.Modules.Relationships.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<CreateRelationshipTemplateCommand>());
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(DomainExceptionTransformerPipelineBehavior<,>));
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateRelationshipTemplateCommandValidator).Assembly);
    }
}
