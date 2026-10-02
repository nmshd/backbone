using CreateFile = Backbone.Modules.Files.Module.Features.Files.CreateFile;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Files.Module.Features.Files.CreateFile;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Files.Module;

internal static class ApplicationServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<CreateFile.Command>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddValidatorsFromAssemblyContaining<Validator>();
    }
}
