using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Challenges.Application.Identities;

namespace Backbone.Job.IdentityDeletion;

public static class ServicesExtensions
{
    public static IServiceCollection RegisterIdentityDeleters(this IServiceCollection services)
    {
        services.AddTransient<IIdentityDeleter, IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Devices.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Files.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Messages.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Quotas.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Relationships.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Synchronization.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Tokens.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Announcements.Application.Identities.IdentityDeleter>();

        return services;
    }
}
