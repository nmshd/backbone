using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipsRepository
{
    Task<bool> RelationshipExists(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken);
    Task<List<Relationship>> GetYoungestRelationships(IdentityAddress mainIdentity, IdentityAddress[] peers, CancellationToken cancellationToken);
}
