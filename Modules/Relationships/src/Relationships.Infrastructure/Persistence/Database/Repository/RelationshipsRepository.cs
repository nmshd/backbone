using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Common;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
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
    private readonly IContentStore _contentStore;

    public RelationshipsRepository(RelationshipsDbContext dbContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, IContentStore contentStore)
    {
        _relationships = dbContext.Relationships;
        _readOnlyRelationships = dbContext.Relationships.AsNoTracking();
        _changes = dbContext.RelationshipChanges;
        _readOnlyChanges = dbContext.RelationshipChanges.AsNoTracking();
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
        _contentStore = contentStore;
    }

    public async Task<DbPaginationResult<RelationshipChange>> FindChangesWithIds(IEnumerable<RelationshipChangeId> ids, RelationshipChangeType? relationshipChangeType, RelationshipChangeStatus? relationshipChangeStatus, OptionalDateRange modifiedAt, OptionalDateRange createdAt, OptionalDateRange completedAt, IdentityAddress createdBy, IdentityAddress completedBy, IdentityAddress identityAddress, PaginationFilter paginationFilter, bool onlyPeerChanges = false, bool track = false)
    {
        var query = (track ? _changes : _readOnlyChanges)
                    .AsQueryable()
                    .IncludeAll()
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

        await Task.WhenAll(changes.ItemsOnPage.Select(FillContentChange).ToArray());

        return changes;
    }

    public async Task<Relationship> FindRelationship(RelationshipId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true)
    {
        var relationship = await(track ? _relationships : _readOnlyRelationships)
                            .IncludeAll()
                            .WithParticipant(identityAddress)
                            .FirstWithId(id, cancellationToken);

        if (fillContent)
        {
            await FillContentChanges(relationship.Changes);
        }

        return relationship;
    }

    public async Task<Relationship> FindRelationshipPlain(RelationshipId id, CancellationToken cancellationToken)
    {
        return await _relationships
                            .IncludeAll()
                            .FirstWithId(id, cancellationToken);
    }

    public async Task<RelationshipChange> FindRelationshipChange(RelationshipChangeId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true)
    {
        var change = await (track ? _changes : _readOnlyChanges)
                            .IncludeAll()
                            .WithId(id)
                            .WithRelationshipParticipant(identityAddress)
                            .FirstOrDefaultAsync(cancellationToken);

        if (fillContent)
        {
            await FillContentChange(change);
        }

        return change;
    }

    public async Task<DbPaginationResult<Relationship>> FindRelationshipsWithIds(IEnumerable<RelationshipId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter, bool track = false)
    {
        var query = (track ? _relationships : _readOnlyRelationships)
                    .AsQueryable()
                    .IncludeAll()
                    .WithParticipant(identityAddress)
                    .WithIdIn(ids);

        var templates = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter);

        await Task.WhenAll(templates.ItemsOnPage.SelectMany(r => r.Changes).Select(FillContentChange).ToArray());

        return templates;
    }

    public async Task Update(Relationship relationship)
    {
        _relationships.Update(relationship);
        await _dbContext.SaveChangesAsync();
    }

    private async Task FillContentChange(RelationshipChange change)
    {
        await _contentStore.FillContentOfChange(change);
    }

    private async Task FillContentChanges(IEnumerable<RelationshipChange> changes)
    {
        await _contentStore.FillContentOfChanges(changes);
    }

    public async Task<RelationshipId> Add(Relationship relationship, CancellationToken cancellationToken)
    {
        var add = await _relationships.AddAsync(relationship, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return add.Entity.Id;
    }

    public async Task<int> CountNumberOfRelationshipsOfTemplate(RelationshipTemplateId relationshipTemplateId, CancellationToken cancellationToken)
    {
        return await _readOnlyRelationships
                    .CountAsync(r => r.RelationshipTemplateId == relationshipTemplateId, cancellationToken);
    }

    public async Task<bool> RelationshipBetweenActiveIdentityAndTemplateOwnerExists(IdentityAddress identityAddress, IdentityAddress createdBy, CancellationToken cancellationToken)
    {
        return await _readOnlyRelationships
                    .BetweenParticipants(identityAddress, createdBy)
                    .Where(r => r.Status != RelationshipStatus.Terminated && r.Status != RelationshipStatus.Rejected && r.Status != RelationshipStatus.Revoked)
                    .AnyAsync(cancellationToken);
    }
}
