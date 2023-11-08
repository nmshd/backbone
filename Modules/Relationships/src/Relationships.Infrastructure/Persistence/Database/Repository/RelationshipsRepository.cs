using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Common;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Backbone.Modules.Relationships.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<RelationshipsRepository> _logger;
    private readonly BlobOptions _blobOptions;

    public RelationshipsRepository(RelationshipsDbContext dbContext, IBlobStorage blobStorage,
        IOptions<BlobOptions> blobOptions, ILogger<RelationshipsRepository> logger)
    {
        _relationships = dbContext.Relationships;
        _readOnlyRelationships = dbContext.Relationships.AsNoTracking();
        _changes = dbContext.RelationshipChanges;
        _readOnlyChanges = dbContext.RelationshipChanges.AsNoTracking();
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _logger = logger;
        _blobOptions = blobOptions.Value;
    }

    public async Task<DbPaginationResult<RelationshipChange>> FindChangesWithIds(IEnumerable<RelationshipChangeId> ids,
        RelationshipChangeType? relationshipChangeType, RelationshipChangeStatus? relationshipChangeStatus,
        OptionalDateRange modifiedAt, OptionalDateRange createdAt, OptionalDateRange completedAt,
        IdentityAddress createdBy, IdentityAddress completedBy, IdentityAddress identityAddress,
        PaginationFilter paginationFilter, CancellationToken cancellationToken, bool onlyPeerChanges = false, bool track = false)
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

        var changes = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        await FillContentOfChanges(changes.ItemsOnPage);

        return changes;
    }

    public async Task<Relationship> FindRelationship(RelationshipId id, IdentityAddress identityAddress,
        CancellationToken cancellationToken, bool track = false, bool fillContent = true)
    {
        var relationship = await (track ? _relationships : _readOnlyRelationships)
            .IncludeAll(_dbContext)
            .WithParticipant(identityAddress)
            .FirstWithId(id, cancellationToken);

        if (fillContent)
        {
            await FillContentOfChanges(relationship.Changes);
        }

        return relationship;
    }

    public async Task<RelationshipChange> FindRelationshipChange(RelationshipChangeId id,
        IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false,
        bool fillContent = true)
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

    public async Task<DbPaginationResult<Relationship>> FindRelationshipsWithIds(IEnumerable<RelationshipId> ids,
        IdentityAddress identityAddress, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool track = false)
    {
        var query = (track ? _relationships : _readOnlyRelationships)
            .AsQueryable()
            .IncludeAll(_dbContext)
            .WithParticipant(identityAddress)
            .WithIdIn(ids);

        var relationships = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        await FillContentOfChanges(relationships.ItemsOnPage.SelectMany(r => r.Changes));

        return relationships;
    }

    public async Task Update(Relationship relationship)
    {
        _relationships.Update(relationship);
        // await SaveContentOfLatestChange(relationship);
        await _dbContext.SaveChangesAsync();
    }


    public async Task Add(Relationship relationship, CancellationToken cancellationToken)
    {
        await _relationships.AddAsync(relationship, cancellationToken);
        // await SaveContentOfLatestChange(relationship);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains(ConstraintNames.ONLY_ONE_ACTIVE_RELATIONSHIP_BETWEEN_TWO_IDENTITIES))
                throw new OperationFailedException(ApplicationErrors.Relationship.RelationshipToTargetAlreadyExists(relationship.To));

            throw;
        }
    }

    public async Task<bool> RelationshipBetweenTwoIdentitiesExists(IdentityAddress identityAddressA,
        IdentityAddress identityAddressB, CancellationToken cancellationToken)
    {
        return await _readOnlyRelationships
            .BetweenParticipants(identityAddressA, identityAddressB)
            .Where(r => r.Status != RelationshipStatus.Terminated && r.Status != RelationshipStatus.Rejected &&
                        r.Status != RelationshipStatus.Revoked)
            .AnyAsync(cancellationToken);
    }

    // private async Task SaveContentOfLatestChange(Relationship relationship)
    // {
    //     var latestChange = relationship.Changes.MaxBy(c => c.CreatedAt);
    //
    //     if (relationship.Status == RelationshipStatus.Pending && latestChange.Request.Content != null)
    //         _blobStorage.Add(_blobOptions.RootFolder, $"{latestChange.Id}_Req", latestChange.Request.Content);
    //     else if (latestChange.Response?.Content != null)
    //         _blobStorage.Add(_blobOptions.RootFolder, $"{latestChange.Id}_Res", latestChange.Response.Content);
    //
    //     try
    //     {
    //         await _blobStorage.SaveAsync();
    //     }
    //     catch (BlobAlreadyExistsException ex)
    //     {
    //         _logger.ErrorTryingToSaveRelationshipChange(latestChange.Id, ex.BlobName);
    //     }
    // }

    private async Task FillContentOfChange(RelationshipChange change)
    {
        if (change.Type == RelationshipChangeType.Creation)
        {
            if (change.Request.Content == null)
            {
                var requestContent = await _blobStorage.FindAsync(_blobOptions.RootFolder, $"{change.Id}_Req");
                change.Request.LoadContent(requestContent);
            }

            if (change.IsCompleted && change.Response!.Content == null)
            {
                try
                {
                    var responseContent = await _blobStorage.FindAsync(_blobOptions.RootFolder, $"{change.Id}_Res");
                    change.Response!.LoadContent(responseContent);
                }
                catch (NotFoundException)
                {
                    // response content is optional so we can ignore this exception
                }
            }
        }
    }

    private async Task FillContentOfChanges(IEnumerable<RelationshipChange> changes)
    {
        await Task.WhenAll(changes.Select(FillContentOfChange).ToArray());
    }
}

internal static partial class RelationshipRepositoryLogs
{
    [LoggerMessage(
        EventId = 664861,
        EventName = "Relationships.RelationshipsRepository.ErrorTryingToSaveRelationshipChange",
        Level = LogLevel.Error,
        Message = "There was an error while trying to save the content of the RelationshipChange with the id '{id}'. The name of the blob was '{name}'.")]
    public static partial void ErrorTryingToSaveRelationshipChange(this ILogger logger, RelationshipChangeId id, string name);
}
