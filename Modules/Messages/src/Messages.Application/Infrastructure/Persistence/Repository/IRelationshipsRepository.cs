using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipsRepository
{
    Task<Relationship?> FindYoungestRelationship(IdentityAddress identityA, IdentityAddress identityB, CancellationToken cancellationToken);
}
