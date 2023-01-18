using Enmeshed.BuildingBlocks.Application.MediatR;
using Files.Application.AutoMapper;
using Files.Application.Files.Commands.CreateFile;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Files.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(CreateFileCommand));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddValidatorsFromAssemblyContaining<CreateFileCommandValidator>();
    }
}
