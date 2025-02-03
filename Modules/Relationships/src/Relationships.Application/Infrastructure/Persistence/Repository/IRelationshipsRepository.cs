using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;

public interface IRelationshipsRepository
{
    Task<DbPaginationResult<Relationship>> FindRelationshipsWithIds(IEnumerable<RelationshipId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter,
        CancellationToken cancellationToken, bool track = false);

    Task<Relationship> FindRelationship(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false);
    Task<IdentityAddress> FindRelationshipPeer(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken);

    Task Add(Relationship relationship, CancellationToken cancellationToken);
    Task Update(Relationship relationship);
    Task Update(IEnumerable<Relationship> relationships);
    Task<IEnumerable<Relationship>> FindRelationships(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken, bool track = false);
    Task<bool> RelationshipBetweenTwoIdentitiesExists(IdentityAddress identityAddressA, IdentityAddress identityAddressB, CancellationToken cancellationToken);
    Task DeleteRelationships(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken);
}
