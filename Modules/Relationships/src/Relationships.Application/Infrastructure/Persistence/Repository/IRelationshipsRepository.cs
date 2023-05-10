﻿using Backbone.Modules.Relationships.Common;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
public interface IRelationshipsRepository
{
    Task<DbPaginationResult<Relationship>> FindRelationshipsWithIds(IEnumerable<RelationshipId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter, bool track = false);
    Task<DbPaginationResult<RelationshipChange>> FindChangesWithIds(IEnumerable<RelationshipChangeId> ids, RelationshipChangeType? relationshipChangeType, RelationshipChangeStatus? relationshipChangeStatus, OptionalDateRange modifiedAt, OptionalDateRange createdAt, OptionalDateRange completedAt, IdentityAddress createdBy, IdentityAddress completedBy, IdentityAddress identityAddress, PaginationFilter paginationFilter, bool onlyPeerChanges = false, bool track = false);
    Task<Relationship> FindRelationship(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true);
    Task<Relationship> FindRelationshipPlain(RelationshipId id, CancellationToken cancellationToken);
    Task<RelationshipChange> FindRelationshipChange(RelationshipChangeId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true);
    Task<RelationshipId> Add(Relationship relationship, CancellationToken cancellationToken);
    Task Update(Relationship relationship);

    Task<bool> RelationshipBetweenTwoIdentitiesExists(IdentityAddress identityAddressA, IdentityAddress identityAddressB, CancellationToken cancellationToken);
    Task<int> CountNumberOfRelationshipsOfTemplate(RelationshipTemplateId relationshipTemplateId, CancellationToken cancellationToken);

    Task SaveContentOfChangeRequest(RelationshipChangeRequest changeRequest);
    Task SaveContentOfChangeResponse(RelationshipChangeResponse changeResponse);
}
