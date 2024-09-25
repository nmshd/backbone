using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Modules.Relationships.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Infrastructure.Persistence.Database.Repository;

public class RelationshipTemplatesRepository : IRelationshipTemplatesRepository
{
    private readonly DbSet<RelationshipTemplate> _templates;
    private readonly IQueryable<RelationshipTemplate> _readOnlyTemplates;
    private readonly DbSet<RelationshipTemplateAllocation> _relationshipTemplateAllocations;
    private readonly RelationshipsDbContext _dbContext;

    public RelationshipTemplatesRepository(RelationshipsDbContext dbContext)
    {
        _templates = dbContext.RelationshipTemplates;
        _readOnlyTemplates = dbContext.RelationshipTemplates.AsNoTracking();
        _relationshipTemplateAllocations = dbContext.RelationshipTemplateAllocations;
        _dbContext = dbContext;
    }

    public async Task Add(RelationshipTemplate template, CancellationToken cancellationToken)
    {
        await _templates.AddAsync(template, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(Expression<Func<RelationshipTemplate, bool>> filter, CancellationToken cancellationToken)
    {
        await _templates.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<RelationshipTemplate?> Find(RelationshipTemplateId id, IdentityAddress identityAddress, CancellationToken cancellationToken, bool track = false)
    {
        var template = await (track ? _templates : _readOnlyTemplates)
            .Include(r => r.Allocations)
            .NotExpiredFor(identityAddress)
            .Where(RelationshipTemplate.CanBeCollectedBy(identityAddress))
            .FirstWithIdOrDefault(id, cancellationToken);

        return template;
    }

    public async Task<DbPaginationResult<RelationshipTemplate>> FindTemplatesWithIds(IEnumerable<RelationshipTemplateQuery> queries, IdentityAddress identityAddress, PaginationFilter paginationFilter,
        CancellationToken cancellationToken, bool track = false)
    {
        var queriesList = queries.ToList();

        Expression<Func<RelationshipTemplate, bool>> filterExpression = template => false;

        foreach (var inputQuery in queriesList)
            filterExpression = filterExpression
                .Or(RelationshipTemplate.HasId(RelationshipTemplateId.Parse(inputQuery.Id))
                    .And(RelationshipTemplate.CanBeCollectedWithPassword(identityAddress, inputQuery.Password))
                    .And(RelationshipTemplate.CanBeCollectedBy(identityAddress)));

        var query = (track ? _templates : _readOnlyTemplates)
            .AsQueryable()
            .NotExpiredFor(identityAddress)
            .Where(filterExpression);

        var templates = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return templates;
    }

    public async Task<DbPaginationResult<RelationshipTemplate>> FindTemplatesWithIds2(IEnumerable<RelationshipTemplateQuery> queries, IdentityAddress identityAddress, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool track = false)
    {
        var queriesList = queries.ToList();

        var query = (track ? _templates : _readOnlyTemplates)
            .AsQueryable()
            .NotExpiredFor(identityAddress)
            .Where(RelationshipTemplate.CanBeCollectedBy(identityAddress))
            .WithIdIn(queriesList.Select(q => RelationshipTemplateId.Parse(q.Id)));

        var templates = await query.ToListAsync(cancellationToken);

        var filteredTemplates = templates
            //.CanBeCollectedWithPassword(identityAddress, queriesList)
            .ToList();

        // move password check to the handler

        var paginatedTemplates = filteredTemplates
            .OrderBy(d => d.CreatedAt)
            .Skip((paginationFilter.PageNumber - 1) * (paginationFilter.PageSize ?? filteredTemplates.Count))
            .Take(paginationFilter.PageSize ?? filteredTemplates.Count)
            .ToList();

        return new DbPaginationResult<RelationshipTemplate>(paginatedTemplates, filteredTemplates.Count);
    }

    public async Task Update(RelationshipTemplate template)
    {
        _templates.Update(template);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<RelationshipTemplateAllocation>> FindRelationshipTemplateAllocations(Expression<Func<RelationshipTemplateAllocation, bool>> filter,
        CancellationToken cancellationToken)
    {
        return await _relationshipTemplateAllocations.Where(filter).ToListAsync(cancellationToken);
    }

    public async Task UpdateRelationshipTemplateAllocations(List<RelationshipTemplateAllocation> templateAllocations, CancellationToken cancellationToken)
    {
        _relationshipTemplateAllocations.UpdateRange(templateAllocations);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
