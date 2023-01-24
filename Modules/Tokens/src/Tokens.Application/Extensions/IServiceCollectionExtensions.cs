using System.Reflection;
using Enmeshed.BuildingBlocks.Application.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Tokens.Application.AutoMapper;
using Tokens.Application.Tokens.Commands.CreateToken;

namespace Tokens.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(CreateTokenCommand).GetTypeInfo().Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateTokenCommandValidator).Assembly);
    }
}
