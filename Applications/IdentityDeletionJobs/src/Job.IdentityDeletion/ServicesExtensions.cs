using Backbone.BuildingBlocks.Application.Identities;

namespace Backbone.Job.IdentityDeletion;

public static class ServicesExtensions
{
    public static IServiceCollection RegisterIdentityDeleters(this IServiceCollection services)
    {
        services.AddTransient<IIdentityDeleter, Modules.Challenges.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Devices.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Files.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Messages.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Quotas.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Relationships.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Synchronization.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Tokens.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Announcements.Application.Announcements.IdentityDeleter>();

        return services;
    }
}
