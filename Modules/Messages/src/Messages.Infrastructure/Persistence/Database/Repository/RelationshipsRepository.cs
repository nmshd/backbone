using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.Repository;

public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly IQueryable<Relationship> _readOnlyRelationships;

    public RelationshipsRepository(MessagesDbContext dbContext)
    {
        _readOnlyRelationships = dbContext.Relationships.AsNoTracking();
    }

    public Task<RelationshipId?> GetIdOfRelationshipBetweenSenderAndRecipient(IdentityAddress sender, IdentityAddress recipient)
    {
        return _readOnlyRelationships
            .WithParticipants(sender, recipient)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();
    }

    public Task<Relationship?> FindYoungestRelationship(IdentityAddress sender, IdentityAddress recipient, CancellationToken cancellationToken)
    {
        return _readOnlyRelationships
            .WithParticipants(sender, recipient)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Relationship>> FindRelationships(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken)
    {
        return await _readOnlyRelationships.Where(filter).ToListAsync(cancellationToken);
    }

    public Task<int> CountActiveRelationships(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken)
    {
        return _readOnlyRelationships
            .Where(filter)
            .CountAsync(cancellationToken);
    }
}
