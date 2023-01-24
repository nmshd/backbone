using System.Reflection;
using Enmeshed.BuildingBlocks.Application.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Relationships.Application.AutoMapper;
using Relationships.Application.MediatR;
using Relationships.Application.Relationships.Commands.CreateRelationshipTemplate;

namespace Relationships.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(CreateRelationshipTemplateCommand).GetTypeInfo().Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainExceptionTransformerPipelineBehavior<,>));
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateRelationshipTemplateCommandValidator).Assembly);
    }
}
