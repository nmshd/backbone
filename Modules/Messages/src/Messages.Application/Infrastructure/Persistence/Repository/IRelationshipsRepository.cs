using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipsRepository
{
    Task<RelationshipId?> GetIdOfRelationshipBetweenSenderAndRecipient(IdentityAddress identityA, IdentityAddress identityB);
}
