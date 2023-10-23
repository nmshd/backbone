using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Files.Application.AutoMapper;
using Backbone.Files.Application.Files.Commands.CreateFile;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Files.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<CreateFileCommand>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddValidatorsFromAssemblyContaining<CreateFileCommandValidator>();
    }
}
