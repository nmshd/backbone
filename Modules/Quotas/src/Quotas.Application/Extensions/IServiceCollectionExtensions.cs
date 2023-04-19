using Backbone.Modules.Quotas.Application.AutoMapper;
using Enmeshed.BuildingBlocks.Application.MediatR;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Quotas.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        // services.AddMediatR(typeof(...)); // needs to be filled as soon as there is the first command/query
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddAutoMapper(typeof(AutoMapperProfile));
        // services.AddValidatorsFromAssemblyContaining<...>(); // needs to be filled as soon as there is the first validator
    }
}