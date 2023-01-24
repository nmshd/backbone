using System.Reflection;
using Enmeshed.BuildingBlocks.Application.MediatR;
using FluentValidation;
using MediatR;
using Messages.Application.AutoMapper;
using Messages.Application.Messages;
using Messages.Application.Messages.Commands.SendMessage;
using Microsoft.Extensions.DependencyInjection;

namespace Messages.Application.Extensions;

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
