using Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;
using Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
public interface IMessagesRepository
{
    Task<DbPaginationResult<Message>> FindMessagesOfIdentity(IdentityAddress identityAddress, ListMessagesQuery request, CancellationToken cancellationToken);
    Task<Message> Find(MessageId id, IdentityAddress address, CancellationToken cancellationToken);
    Task<MessageId> Add(Message message, CancellationToken cancellationToken);
    Task<int> CountUnreceivedMessagesFromActiveIdentity(IdentityAddress sender, SendMessageCommandRecipientInformation recipientDto, CancellationToken cancellationToken);
    EntityEntry<Message> Update(Message message);
}
