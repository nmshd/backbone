using System.Reflection;
using Backbone.Modules.Messages.Application.AutoMapper;
using Backbone.Modules.Messages.Application.Messages;
using Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;
using Enmeshed.BuildingBlocks.Application.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Messages.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(SendMessageCommand).GetTypeInfo().Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(SendMessageCommandValidator).Assembly);

        services.AddTransient<MessageService>();
    }
}
