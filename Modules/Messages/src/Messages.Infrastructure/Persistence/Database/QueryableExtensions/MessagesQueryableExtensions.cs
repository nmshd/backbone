using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.QueryableExtensions;

public static class MessagesQueryableExtensions
{
    public static async Task<Message> FirstWithId(this IQueryable<Message> query, MessageId id, CancellationToken cancellationToken)
    {
        var message = await query.FirstOrDefaultAsync(m => m.Id == id, cancellationToken) ?? throw new NotFoundException(nameof(Message));
        return message;
    }

    public static IQueryable<Message> WithIdsIn(this IQueryable<Message> query, IEnumerable<MessageId> ids)
    {
        return query.Where(m => ids.Contains(m.Id));
    }

    public static IQueryable<Message> WithASpecificRecipientWhoDidNotReceiveTheMessage(this IQueryable<Message> query, IdentityAddress recipient)
    {
        return query.Where(m => m.Recipients.Any(r => r.Address == recipient && !r.ReceivedAt.HasValue));
    }

    public static IQueryable<Message> FromASpecificSender(this IQueryable<Message> query, IdentityAddress sender)
    {
        return query.Where(m => m.CreatedBy == sender);
    }
}
