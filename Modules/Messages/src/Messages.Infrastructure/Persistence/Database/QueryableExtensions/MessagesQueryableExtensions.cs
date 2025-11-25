using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.QueryableExtensions;

public static class MessagesQueryableExtensions
{
    extension(IQueryable<Message> query)
    {
        public async Task<Message> FirstWithId(MessageId id, CancellationToken cancellationToken)
        {
            var message = await query.FirstOrDefaultAsync(m => m.Id == id, cancellationToken) ?? throw new NotFoundException(nameof(Message));
            return message;
        }

        public IQueryable<Message> WithIdsIn(IEnumerable<MessageId> ids)
        {
            return query.Where(m => ids.Contains(m.Id));
        }

        public IQueryable<Message> WithASpecificRecipientWhoDidNotReceiveTheMessage(IdentityAddress recipient)
        {
            return query.Where(m => m.Recipients.Any(r => r.Address == recipient && !r.ReceivedAt.HasValue));
        }

        public IQueryable<Message> FromASpecificSender(IdentityAddress sender)
        {
            return query.Where(m => m.CreatedBy == sender);
        }
    }
}
