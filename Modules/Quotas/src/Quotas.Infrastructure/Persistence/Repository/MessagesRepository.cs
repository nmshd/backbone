using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Messages;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class MessagesRepository : IMessagesRepository
{
    private readonly IQueryable<Message> _readOnlyMessages;

    public MessagesRepository(QuotasDbContext dbContext)
    {
        _readOnlyMessages = dbContext.Messages.AsNoTracking();
    }

    public async Task<uint> Count(IdentityAddress sender, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
    {
        var count = await _readOnlyMessages.CountAsync(m => m.CreatedBy == sender.StringValue, cancellationToken);
        return (uint)count;
    }
}
