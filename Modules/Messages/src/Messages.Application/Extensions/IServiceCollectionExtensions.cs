using Backbone.Modules.Messages.Application.AutoMapper;
using Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;
using Enmeshed.BuildingBlocks.Application.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Backbone.Modules.Messages.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<SendMessageCommand>());
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(SendMessageCommandValidator).Assembly);
    }
}
