using Backbone.BuildingBlocks.Application.Identities;

namespace Backbone.Job.IdentityDeletion;

public static class ServicesExtensions
{
    public static IServiceCollection RegisterIdentityDeleters(this IServiceCollection services)
    {
        services.AddTransient<IIdentityDeleter, Modules.Announcements.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Challenges.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Devices.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Files.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Messages.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Quotas.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Relationships.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Synchronization.Application.Identities.IdentityDeleter>();
        services.AddTransient<IIdentityDeleter, Modules.Tokens.Module.Features.Identities.DeleteIdentity.IdentityDeleter>();

        return services;
    }
}
