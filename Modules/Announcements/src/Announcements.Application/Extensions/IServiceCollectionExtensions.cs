using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backbone.Modules.Announcements.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<CreateAnnouncementCommand>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(Validator).Assembly);

        services.AddHousekeeper<Housekeeper>();
    }
}
