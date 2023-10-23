using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Relationships.Common;
using Backbone.Relationships.Domain.Entities;
using Backbone.Relationships.Domain.Ids;

namespace Backbone.Relationships.Application.Infrastructure.Persistence.Repository;
public interface IRelationshipsRepository
{
    Task<DbPaginationResult<Relationship>> FindRelationshipsWithIds(IEnumerable<RelationshipId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool track = false);
    Task<DbPaginationResult<RelationshipChange>> FindChangesWithIds(IEnumerable<RelationshipChangeId> ids, RelationshipChangeType? relationshipChangeType,
        RelationshipChangeStatus? relationshipChangeStatus, OptionalDateRange modifiedAt, OptionalDateRange createdAt, OptionalDateRange completedAt, IdentityAddress createdBy,
        IdentityAddress completedBy, IdentityAddress identityAddress, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool onlyPeerChanges = false, bool track = false);
    Task<Relationship> FindRelationship(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true);
    Task<RelationshipChange> FindRelationshipChange(RelationshipChangeId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true);
    Task Add(Relationship relationship, CancellationToken cancellationToken);
    Task Update(Relationship relationship);

    Task<bool> RelationshipBetweenTwoIdentitiesExists(IdentityAddress identityAddressA, IdentityAddress identityAddressB, CancellationToken cancellationToken);
}
