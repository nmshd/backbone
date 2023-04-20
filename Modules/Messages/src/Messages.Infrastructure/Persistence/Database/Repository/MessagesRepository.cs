using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Microsoft.EntityFrameworkCore;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.Repository;
public class MessagesRepository : IMessagesRepository
{
    private readonly DbSet<Message> _messages;
    private readonly IQueryable<Message> _readOnlyMessages;
    private readonly IUserContext _userContext;

    public MessagesRepository(MessagesDbContext dbContext, IUserContext userContext)
    {
        _messages = dbContext.Messages;
        _readOnlyMessages = dbContext.Messages.AsNoTracking();
        _userContext = userContext;
    }
    async Task<Message> IMessagesRepository.Find(MessageId id, CancellationToken cancellationToken)
    {
        return await _readOnlyMessages
            .IncludeAllReferences()
            .WithSenderOrRecipient(_userContext.GetAddress())
            .FirstWithId(id, cancellationToken);
    }

    async Task<DbPaginationResult<Message>> IMessagesRepository.FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        return await _readOnlyMessages.OrderAndPaginate(d => d.CreatedAt, paginationFilter);
    }

    public async Task<MessageId> Add(Message message, CancellationToken cancellationToken)
    {
        var add = await _messages.AddAsync(message, cancellationToken);
        return add.Entity.Id;
    }

    public Task<int> CountUnreceivedMessagesFromActiveIdentity(IdentityAddress sender, SendMessageCommandRecipientInformation recipientDto, CancellationToken cancellationToken)
    {
        return _readOnlyMessages
            .FromASpecificSender(sender)
            .WithASpecificRecipientWhoDidNotReceiveTheMessage(recipientDto.Address)
            .CountAsync(cancellationToken);
    }
}
