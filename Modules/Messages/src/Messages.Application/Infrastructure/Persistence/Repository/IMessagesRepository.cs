using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
public interface IMessagesRepository
{
    Task<DbPaginationResult<Message>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task<Message> Find(MessageId id, CancellationToken cancellationToken);
    Task<MessageId> Add(Message message, CancellationToken cancellationToken);
}
