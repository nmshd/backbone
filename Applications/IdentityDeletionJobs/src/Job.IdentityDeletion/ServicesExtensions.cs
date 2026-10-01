using Backbone.BuildingBlocks.Application.Identities;

namespace Backbone.Job.IdentityDeletion;

public static class ServicesExtensions
{
    public static IServiceCollection RegisterIdentityDeleters(this IServiceCollection services)
    {
        services.AddTransient<IIdentityDeleter, Modules.Announcements.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Challenges.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Devices.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Files.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Messages.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Quotas.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Relationships.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Synchronization.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Tokens.Application.Identities.IdentityDeleter>();

        return services;
    }
}
