using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Tokens.Module.Features.Tokens.CreateToken;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Module;

internal static class ApplicationServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<CreateTokenCommand>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
            .AddOpenBehavior(typeof(DbConcurrencyBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(Validator).Assembly);
    }
}
