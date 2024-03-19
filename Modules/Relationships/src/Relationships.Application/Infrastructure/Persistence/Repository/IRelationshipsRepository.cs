using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Common;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
public interface IRelationshipsRepository
{
    #region Relationships

    Task<DbPaginationResult<Relationship>> FindRelationshipsWithIds(IEnumerable<RelationshipId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool track = false);
    Task Add(Relationship relationship, CancellationToken cancellationToken);
    Task Update(Relationship relationship);
    Task<IEnumerable<Relationship>> FindRelationships(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken);
    Task<bool> RelationshipBetweenTwoIdentitiesExists(IdentityAddress identityAddressA, IdentityAddress identityAddressB, CancellationToken cancellationToken);
    Task Delete(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken);
    
    #endregion Relationships

    #region RelationshipChanges

    Task<Relationship> FindRelationship(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false);
    Task<DbPaginationResult<RelationshipChange>> FindChangesWithIds(IEnumerable<RelationshipChangeId> ids, RelationshipChangeType? relationshipChangeType,
        RelationshipChangeStatus? relationshipChangeStatus, OptionalDateRange? modifiedAt, OptionalDateRange? createdAt, OptionalDateRange? completedAt, IdentityAddress? createdBy,
        IdentityAddress? completedBy, IdentityAddress activeIdentity, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool onlyPeerChanges = false, bool track = false);
    Task<RelationshipChange?> FindRelationshipChange(RelationshipChangeId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false);
    
    #endregion RelationshipChanges

    #region RelationshipTemplateAllocations

    Task<IEnumerable<RelationshipTemplateAllocation>> FindRelationshipTemplateAllocations(Expression<Func<RelationshipTemplateAllocation, bool>> filter, CancellationToken cancellationToken);
    Task UpdateRelationshipTemplateAllocation(RelationshipTemplateAllocation relationshipTemplateAllocation, CancellationToken cancellationToken);
   
    #endregion RelationshipTemplateAllocations
}
