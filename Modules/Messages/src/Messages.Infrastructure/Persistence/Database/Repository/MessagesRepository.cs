using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Microsoft.EntityFrameworkCore;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.Repository;
public class MessagesRepository : IMessagesRepository
{
    private readonly DbSet<Message> _messages;
    private readonly IQueryable<Message> _readOnlyMessages;
    private readonly MessagesDbContext _dbContext;

    public MessagesRepository(MessagesDbContext dbContext)
    {
        _messages = dbContext.Messages;
        _readOnlyMessages = dbContext.Messages.AsNoTracking();
        _dbContext = dbContext;
    }
    public async Task<Message> Find(MessageId id, IdentityAddress address, CancellationToken cancellationToken)
    {
        return await _readOnlyMessages
            .IncludeAllReferences()
            .WithSenderOrRecipient(address)
            .FirstWithId(id, cancellationToken);
    }

    public async Task<Message> FindPlain(MessageId id, CancellationToken cancellationToken)
    {
        return await _readOnlyMessages.FirstWithId(id, cancellationToken);
    }

    public async Task<DbPaginationResult<Message>> FindAll(PaginationFilter paginationFilter)
    {
        return await _readOnlyMessages.OrderAndPaginate(d => d.CreatedAt, paginationFilter);
    }

    public async Task<MessageId> Add(Message message, CancellationToken cancellationToken)
    {
        var add = await _messages.AddAsync(message, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return add.Entity.Id;
    }

    public Task<int> CountUnreceivedMessagesFromActiveIdentity(IdentityAddress sender, SendMessageCommandRecipientInformation recipientDto, CancellationToken cancellationToken)
    {
        return _readOnlyMessages
            .FromASpecificSender(sender)
            .WithASpecificRecipientWhoDidNotReceiveTheMessage(recipientDto.Address)
            .CountAsync(cancellationToken);
    }

    public Task<DbPaginationResult<Message>> FindMessagesOfIdentity(IdentityAddress identityAddress, ListMessagesQuery request, IdentityAddress addressOfActiveIdentity, CancellationToken cancellationToken)
    {
        var query = _readOnlyMessages
            .AsQueryable()
            .IncludeAllReferences();

        if (request.Ids.Any())
            query = query.WithIdsIn(request.Ids);

        query = query.WithSenderOrRecipient(identityAddress);
        query = query.DoNotSendBeforePropertyIsNotInTheFuture();

        return query.OrderAndPaginate(d => d.CreatedAt, request.PaginationFilter);
    }

    public EntityEntry<Message> Update(Message message)
    {
        var m = _messages.Update(message);
        _dbContext.SaveChanges();
        return m;
    }
}
