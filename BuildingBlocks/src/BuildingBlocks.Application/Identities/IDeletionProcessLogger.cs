using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.Identities;
public interface IDeletionProcessLogger
{
    Task LogDeletion(IdentityAddress identityAddress, AggregateType aggregateType);
}
