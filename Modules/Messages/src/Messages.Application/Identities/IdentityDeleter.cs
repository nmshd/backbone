using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Messages.Application.Identities;

public class IdentityDeleter : IIdentityDeleter
{
    public Task Delete(IdentityAddress identityAddress)
    {
        return Task.CompletedTask;
    }
}
