using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfigurationSection applicationConfiguration)
    {
        services.ConfigureAndValidate<ApplicationOptions>(applicationConfiguration.Bind);

        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<CreateTokenCommand>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(Validator).Assembly);
    }
}
