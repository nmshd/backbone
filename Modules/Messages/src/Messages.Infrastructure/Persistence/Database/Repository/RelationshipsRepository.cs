using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Messages.Domain.Ids;
using Backbone.Messages.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Messages.Infrastructure.Persistence.Database.Repository;
public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly MessagesDbContext _dbContext;

    public RelationshipsRepository(MessagesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

#nullable enable
    public Task<RelationshipId?> GetIdOfRelationshipBetweenSenderAndRecipient(IdentityAddress sender, IdentityAddress recipient)
    {
        return _dbContext.Relationships
            .AsNoTracking()
            .WithParticipants(sender, recipient)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();
    }
#nullable disable
}
