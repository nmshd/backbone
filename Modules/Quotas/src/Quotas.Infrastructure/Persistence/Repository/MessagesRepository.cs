using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Quotas.Domain.Aggregates.Messages;
using Backbone.Quotas.Infrastructure.Persistence.Database;
using Backbone.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Quotas.Infrastructure.Persistence.Repository;
public class MessagesRepository : IMessagesRepository
{
    private readonly IQueryable<Message> _readOnlyMessages;

    public MessagesRepository(QuotasDbContext dbContext)
    {
        _readOnlyMessages = dbContext.Messages.AsNoTracking();
    }

    public async Task<uint> Count(IdentityAddress sender, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
    {
        var count = await _readOnlyMessages
            .CreatedInInterval(createdAtFrom, createdAtTo)
            .CountAsync(m => m.CreatedBy == sender.StringValue, cancellationToken);
        return (uint)count;
    }
}
