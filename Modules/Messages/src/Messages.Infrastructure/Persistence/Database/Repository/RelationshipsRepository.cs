using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.Repository;
public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly MessagesDbContext _dbContext;

    public RelationshipsRepository(MessagesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RelationshipId?> GetIdOfRelationshipBetweenSenderAndRecipient(IdentityAddress sender, IdentityAddress recipient)
    {
        return _dbContext.Relationships
            .AsNoTracking()
            .WithParticipants(sender, recipient)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();
    }

    public Task<Relationship?> FindRelationship(IdentityAddress sender, IdentityAddress recipient, CancellationToken cancellationToken)
    {
        return _dbContext.Relationships
            .AsNoTracking()
            .WithParticipants(sender, recipient)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
