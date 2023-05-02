using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;

#nullable enable
public interface IRelationshipsRepository
{
    Task<RelationshipId?> GetIdOfRelationshipBetweenSenderAndRecipient(IdentityAddress identityA, IdentityAddress identityB);
}
