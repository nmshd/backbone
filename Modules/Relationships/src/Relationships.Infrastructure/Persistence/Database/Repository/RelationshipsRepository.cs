using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
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

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.Repository;

public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly DbSet<Relationship> _relationships;
    private readonly DbSet<RelationshipChange> _changes;
    private readonly IQueryable<Relationship> _readOnlyRelationships;
    private readonly IQueryable<RelationshipChange> _readOnlyChanges;
    private readonly RelationshipsDbContext _dbContext;
    private readonly ILogger<RelationshipsRepository> _logger;

    public RelationshipsRepository(RelationshipsDbContext dbContext, ILogger<RelationshipsRepository> logger)
    {
        _relationships = dbContext.Relationships;
        _readOnlyRelationships = dbContext.Relationships.AsNoTracking();
        _changes = dbContext.RelationshipChanges;
        _readOnlyChanges = dbContext.RelationshipChanges.AsNoTracking();
        _dbContext = dbContext;
        _logger = logger;
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

        return changes;
    }

    public async Task<Relationship> FindRelationship(RelationshipId id, IdentityAddress identityAddress,
        CancellationToken cancellationToken, bool track = false)
    {
        var relationship = await (track ? _relationships : _readOnlyRelationships)
            .IncludeAll(_dbContext)
            .WithParticipant(identityAddress)
            .FirstWithId(id, cancellationToken);

        return relationship;
    }

    public async Task<RelationshipChange> FindRelationshipChange(RelationshipChangeId id,
        IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false)
    {
        var change = await (track ? _changes : _readOnlyChanges)
            .IncludeAll(_dbContext)
            .WithId(id)
            .WithRelationshipParticipant(identityAddress)
            .FirstOrDefaultAsync(cancellationToken);

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

    public async Task<IEnumerable<Relationship>> FindRelationships(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken)
    {
        return await _relationships.Where(filter).ToListAsync(cancellationToken);
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
