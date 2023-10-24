using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;

#nullable enable
public interface IRelationshipsRepository
{
    Task<RelationshipId?> GetIdOfRelationshipBetweenSenderAndRecipient(IdentityAddress identityA, IdentityAddress identityB);
}
