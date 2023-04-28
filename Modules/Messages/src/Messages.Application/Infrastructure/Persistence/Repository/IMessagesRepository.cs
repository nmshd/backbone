using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
public interface IMessagesRepository
{
    Task<DbPaginationResult<Message>> FindMessagesWithIds(IEnumerable<MessageId> ids, IdentityAddress requiredParticipant, PaginationFilter paginationFilter, bool track = false);
    Task<Message> Find(MessageId id, IdentityAddress requiredParticipant, CancellationToken cancellationToken, bool track = false, bool noBody = false);
    Task<MessageId> Add(Message message, CancellationToken cancellationToken);
    Task<int> CountUnreceivedMessagesFromSenderToRecipient(IdentityAddress sender, IdentityAddress recipient, CancellationToken cancellationToken);
    Task Update(Message message);
    Task Update(IEnumerable<Message> messages);
}
