using Backbone.Modules.Relationships.Application.Extensions;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
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
    private readonly DbSet<RelationshipTemplate> _templates;
    private readonly DbSet<RelationshipChange> _changes;
    private readonly IQueryable<RelationshipTemplate> _readOnlyTemplates;
    private readonly IQueryable<RelationshipChange> _readOnlyChanges;
    private readonly RelationshipsDbContext _dbContext;
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;
    private readonly IContentStore _contentStore;

    public RelationshipsRepository(RelationshipsDbContext dbContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, IContentStore contentStore)
    {
        _templates = dbContext.RelationshipTemplates;
        _readOnlyTemplates = dbContext.RelationshipTemplates.AsNoTracking();
        _changes = dbContext.RelationshipChanges;
        _readOnlyChanges = dbContext.RelationshipChanges.AsNoTracking();
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
        _contentStore = contentStore;
    }

    public async Task<RelationshipTemplateId> AddRelationshipTemplate(RelationshipTemplate template, CancellationToken cancellationToken)
    {
        await _contentStore.SaveContentOfTemplate(template);

        var add = await _templates.AddAsync(template, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return add.Entity.Id;
    }

    public async Task<RelationshipChange> FindRelationshipChange(RelationshipChangeId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true)
    {
        // TODO: Missing .IncludeAll()
        var change = await (track ? _changes : _readOnlyChanges)
                            .WithId(id)
                            .WithRelationshipParticipant(identityAddress)
                            .FirstOrDefaultAsync(cancellationToken);

        if (fillContent)
        {
            await FillContentChange(change);
        }

        return change;
    }

    public async Task<RelationshipTemplate> FindRelationshipTemplate(RelationshipTemplateId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true)
    {
        var template = await (track ? _templates : _readOnlyTemplates)
                    .Include(r => r.Allocations)
                    .NotExpiredFor(identityAddress)
                    .NotDeleted()
                    .FirstWithId(id, cancellationToken);

        if (fillContent)
        {
            await FillContentTemplate(template);
        }

        return template;
    }

    public async Task<DbPaginationResult<RelationshipTemplate>> FindTemplatesWithIds(IEnumerable<RelationshipTemplateId> ids, IdentityAddress identityAddress, PaginationFilter paginationFilter, bool track = false)
    {
        var query = (track ? _templates : _readOnlyTemplates)
                    .AsQueryable()
                    .NotExpiredFor(identityAddress)
                    .NotDeleted()
                    .WithIdIn(ids);

        var templates = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter);

        await Task.WhenAll(templates.ItemsOnPage.Select(FillContentTemplate).ToArray());

        return templates;
    }

    public async Task UpdateRelationshipTemplate(RelationshipTemplate template)
    {
        _templates.Update(template);
        await _dbContext.SaveChangesAsync();
    }

    private async Task FillContentChange(RelationshipChange change)
    {
        await _contentStore.FillContentOfChange(change);
    }

    private async Task FillContentTemplate(RelationshipTemplate template)
    {
        await _contentStore.FillContentOfTemplate(template);
    }
}
