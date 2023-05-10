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
public class RelationshipTemplatesRepository : IRelationshipTemplatesRepository
{
    private readonly DbSet<RelationshipTemplate> _templates;
    private readonly IQueryable<RelationshipTemplate> _readOnlyTemplates;
    private readonly RelationshipsDbContext _dbContext;
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;

    public RelationshipTemplatesRepository(RelationshipsDbContext dbContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions)
    {
        _templates = dbContext.RelationshipTemplates;
        _readOnlyTemplates = dbContext.RelationshipTemplates.AsNoTracking();
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
    }

    public async Task Add(RelationshipTemplate template, CancellationToken cancellationToken)
    {
        await SaveContentOfTemplate(template);

        await _templates.AddAsync(template, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RelationshipTemplate> Find(RelationshipTemplateId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false, bool fillContent = true)
    {
        var template = await (track ? _templates : _readOnlyTemplates)
                    .Include(r => r.Allocations)
                    .NotExpiredFor(identityAddress)
                    .NotDeleted()
                    .FirstWithId(id, cancellationToken);

        if (fillContent)
        {
            await FillContentOfTemplate(template);
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

        await Task.WhenAll(templates.ItemsOnPage.Select(FillContentOfTemplate).ToArray());

        return templates;
    }

    public async Task Update(RelationshipTemplate template)
    {
        _templates.Update(template);
        await _dbContext.SaveChangesAsync();
    }

    private async Task FillContentOfTemplate(RelationshipTemplate template)
    {
        var content = await _blobStorage.FindAsync(_blobOptions.RootFolder, template.Id);
        template.LoadContent(content);
    }

    private async Task FillContentOfTemplates(IEnumerable<RelationshipTemplate> templates)
    {
        await Task.WhenAll(templates.Select(FillContentOfTemplate).ToArray());
    }

    private async Task SaveContentOfTemplate(RelationshipTemplate template)
    {
        if (template.Content == null)
            return;

        _blobStorage.Add(_blobOptions.RootFolder, template.Id, template.Content);
        await _blobStorage.SaveAsync();
    }
}
