using Backbone.Modules.Files.Application.AutoMapper;
using Backbone.Modules.Files.Application.Files.Commands.CreateFile;
using Enmeshed.BuildingBlocks.Application.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Files.Application.Extensions;

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
