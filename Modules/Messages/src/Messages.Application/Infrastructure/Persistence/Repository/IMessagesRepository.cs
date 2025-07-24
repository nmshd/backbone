using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;

namespace Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;

public interface IMessagesRepository
{
    Task<DbPaginationResult<Message>> ListWithContent(IEnumerable<MessageId> ids, IdentityAddress requiredParticipant, PaginationFilter paginationFilter, CancellationToken cancellationToken,
        bool track = false);

    Task<Message> GetWithContent(MessageId id, IdentityAddress requiredParticipant, CancellationToken cancellationToken, bool track = false);
    Task Add(Message message, CancellationToken cancellationToken);
    Task<int> CountUnreceivedMessagesFromSenderToRecipient(IdentityAddress sender, IdentityAddress recipient, CancellationToken cancellationToken);
    Task Update(Message message);
    Task Update(IEnumerable<Message> messages);
    Task<IEnumerable<Message>> ListWithoutContent(Expression<Func<Message, bool>> expression, CancellationToken cancellationToken);
    Task Delete(MessageId messageId, CancellationToken cancellationToken);
}
