using CreateAnnouncement = Backbone.Modules.Announcements.Module.Features.Announcements.CreateAnnouncement;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Announcements.Module.Features.Announcements.CreateAnnouncement;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Announcements.Module;

internal static class ApplicationServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<CreateAnnouncement.Command>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(Validator).Assembly);
    }
}
