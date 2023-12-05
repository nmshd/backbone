using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Messages.Application.AutoMapper;
using Backbone.Modules.Messages.Application.Identities;
using Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Messages.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<SendMessageCommand>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(SendMessageCommandValidator).Assembly);

        services.AddSingleton<IIdentityDeleter, IdentityDeleter>();
    }
}
