using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Tokens.Application.AutoMapper;
using Backbone.Modules.Tokens.Application.Identities;
using Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Tokens.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
                    .RegisterServicesFromAssemblyContaining<CreateTokenCommand>()
                    .AddOpenBehavior(typeof(LoggingBehavior<,>))
                    .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
                    .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
                );
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateTokenCommandValidator).Assembly);

        services.AddSingleton<IIdentityDeleter, IdentityDeleter>();
    }
}
