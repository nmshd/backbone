using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
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

    public Task<Relationship?> GetYoungestRelationship(IdentityAddress sender, IdentityAddress recipient, CancellationToken cancellationToken)
    {
        return _readOnlyRelationships
            .WithParticipants(sender, recipient)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
