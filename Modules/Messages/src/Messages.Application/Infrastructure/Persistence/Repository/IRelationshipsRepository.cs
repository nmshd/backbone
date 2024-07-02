using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipsRepository
{
    Task<Relationship?> FindYoungestRelationship(IdentityAddress identityA, IdentityAddress identityB, CancellationToken cancellationToken);
    Task<IEnumerable<Relationship>> FindRelationships(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken);
    Task<int> CountActiveRelationships(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken);
}
