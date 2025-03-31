using Backbone.BuildingBlocks.Application.Identities;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Messages.Application.Identities;

public class IdentityDeleter : IIdentityDeleter
{
    // we current don't have to do anything here, because the messages are anonymized automatically by the RelationshipStatusChangedDomainEventHandler
    // once the relationship gets decomposed (which happens during the identity deletion)
    public Task Delete(IdentityAddress identityAddress)
    {
        return Task.CompletedTask;
    }
}
