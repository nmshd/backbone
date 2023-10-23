using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Messages.Domain.Ids;

namespace Backbone.Messages.Application.Infrastructure.Persistence.Repository;

#nullable enable
public interface IRelationshipsRepository
{
    Task<RelationshipId?> GetIdOfRelationshipBetweenSenderAndRecipient(IdentityAddress identityA, IdentityAddress identityB);
}
