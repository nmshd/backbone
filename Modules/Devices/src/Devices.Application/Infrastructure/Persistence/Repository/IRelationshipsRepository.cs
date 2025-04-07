using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipsRepository
{
    Task<bool> RelationshipExistsBetween(IdentityAddress identityAddress1, IdentityAddress identityAddress2);
}
