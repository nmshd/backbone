using CreateChallenge = Backbone.Modules.Challenges.Module.Features.Challenges.CreateChallenge;
using Backbone.BuildingBlocks.Application.MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Challenges.Module;

internal static class ApplicationServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<CreateChallenge.Command>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(CreateChallenge.Validator).Assembly);
    }
}
