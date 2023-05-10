﻿using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Common;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Backbone.Modules.Relationships.Infrastructure.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.Repository;
public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly DbSet<Relationship> _relationships;
    private readonly DbSet<RelationshipChange> _changes;
    private readonly IQueryable<Relationship> _readOnlyRelationships;
    private readonly IQueryable<RelationshipChange> _readOnlyChanges;
    private readonly RelationshipsDbContext _dbContext;
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;

    public RelationshipsRepository(RelationshipsDbContext dbContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions)
    {
        _relationships = dbContext.Relationships;
        _readOnlyRelationships = dbContext.Relationships.AsNoTracking();
        _changes = dbContext.RelationshipChanges;
        _readOnlyChanges = dbContext.RelationshipChanges.AsNoTracking();
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
    }

    public async Task<DbPaginationResult<RelationshipChange>> FindChangesWithIds(IEnumerable<RelationshipChangeId> ids, RelationshipChangeType? relationshipChangeType, RelationshipChangeStatus? relationshipChangeStatus, OptionalDateRange modifiedAt, OptionalDateRange createdAt, OptionalDateRange completedAt, IdentityAddress createdBy, IdentityAddress completedBy, IdentityAddress identityAddress, PaginationFilter paginationFilter, bool onlyPeerChanges = false, bool track = false)
    {
        var query = (track ? _changes : _readOnlyChanges)
                    .AsQueryable()
                    .IncludeAll(_dbContext)
                    .WithType(relationshipChangeType)
                    .WithStatus(relationshipChangeStatus)
                    .ModifiedAt(modifiedAt)
                    .CreatedAt(createdAt)
                    .CompletedAt(completedAt)
                    .CreatedBy(createdBy)
                    .CompletedBy(completedBy)
                    .WithRelationshipParticipant(identityAddress);

        if (ids.Any())
            query = query.WithIdIn(ids);

        if (onlyPeerChanges)
            query = query.OnlyPeerChanges(identityAddress);

        var changes = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter);

        await FillContentOfChanges(changes.ItemsOnPage);

        return changes;
    }

    public async Task<Relationship> FindRelationship(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true)
    {
        var relationship = await(track ? _relationships : _readOnlyRelationships)
                            .IncludeAll(_dbContext)
                            .WithParticipant(identityAddress)
                            .FirstWithId(id, cancellationToken);

        if (fillContent)
        {
            await FillContentOfChanges(relationship.Changes);
        }

        return relationship;
    }

    public async Task<RelationshipChange> FindRelationshipChange(RelationshipChangeId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true)
    {
        var change = await (track ? _changes : _readOnlyChanges)
                            .IncludeAll(_dbContext)
                            .WithId(id)
                            .WithRelationshipParticipant(identityAddress)
                            .FirstOrDefaultAsync(cancellationToken);

        if (fillContent)
        {
            await FillContentOfChange(change);
        }

        return change;
    }

    public async Task<DbPaginationResult<Relationship>> FindRelationshipsWithIds(IEnumerable<RelationshipId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter, bool track = false)
    {
        var query = (track ? _relationships : _readOnlyRelationships)
                    .AsQueryable()
                    .IncludeAll(_dbContext)
                    .WithParticipant(identityAddress)
                    .WithIdIn(ids);

        var relationships = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter);

        await FillContentOfChanges(relationships.ItemsOnPage.SelectMany(r => r.Changes));

        return relationships;
    }

    public async Task Update(Relationship relationship)
    {
        _relationships.Update(relationship);
        await _dbContext.SaveChangesAsync();
    }


    public async Task Add(Relationship relationship, CancellationToken cancellationToken)
    {
        await _relationships.AddAsync(relationship, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RelationshipBetweenTwoIdentitiesExists(IdentityAddress identityAddressA, IdentityAddress identityAddressB, CancellationToken cancellationToken)
    {
        return await _readOnlyRelationships
                    .BetweenParticipants(identityAddressA, identityAddressB)
                    .Where(r => r.Status != RelationshipStatus.Terminated && r.Status != RelationshipStatus.Rejected && r.Status != RelationshipStatus.Revoked)
                    .AnyAsync(cancellationToken);
    }

    public async Task SaveContentOfChangeRequest(RelationshipChangeRequest changeRequest)
    {
        if (changeRequest.Content == null)
            return;

        _blobStorage.Add(_blobOptions.RootFolder, $"{changeRequest.Id}_Req", changeRequest.Content);
        await _blobStorage.SaveAsync();
    }

    public async Task SaveContentOfChangeResponse(RelationshipChangeResponse changeResponse)
    {
        if (changeResponse.Content == null)
            return;

        _blobStorage.Add(_blobOptions.RootFolder, $"{changeResponse.Id}_Res", changeResponse.Content);
        await _blobStorage.SaveAsync();
    }

    private async Task FillContentOfChange(RelationshipChange change)
    {
        if (change.Type == RelationshipChangeType.Creation)
        {
            var requestContent = await _blobStorage.FindAsync(_blobOptions.RootFolder, $"{change.Id}_Req");
            change.Request.LoadContent(requestContent);

            if (change.IsCompleted)
            {
                var responseContent = await _blobStorage.FindAsync(_blobOptions.RootFolder, $"{change.Id}_Res");
                change.Response!.LoadContent(responseContent);
            }
        }
    }

    private async Task FillContentOfChanges(IEnumerable<RelationshipChange> changes)
    {
        await Task.WhenAll(changes.Select(FillContentOfChange).ToArray());
    }
}
