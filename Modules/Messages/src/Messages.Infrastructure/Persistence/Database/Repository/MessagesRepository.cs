using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Message> Find(MessageId id, IdentityAddress address, CancellationToken cancellationToken, bool track = false, bool fillBody = true)
    {
        var message = await (track ? _messages : _readOnlyMessages)
            .IncludeAll(_dbContext)
            .WithSenderOrRecipient(address)
            .FirstWithId(id, cancellationToken);

        return message;
    }

    public async Task Add(Message message, CancellationToken cancellationToken)
    {
        await _messages.AddAsync(message, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountUnreceivedMessagesFromSenderToRecipient(IdentityAddress sender, IdentityAddress recipient, CancellationToken cancellationToken)
    {
        return await _readOnlyMessages
            .FromASpecificSender(sender)
            .WithASpecificRecipientWhoDidNotReceiveTheMessage(recipient)
            .CountAsync(cancellationToken);
    }

    public async Task<DbPaginationResult<Message>> FindMessagesWithIds(IEnumerable<MessageId> ids, IdentityAddress requiredParticipant, PaginationFilter paginationFilter,
        CancellationToken cancellationToken, bool track = false)
    {
        var query = (track ? _messages : _readOnlyMessages)
            .AsQueryable()
            .IncludeAll(_dbContext);

        if (ids.Any())
            query = query.WithIdsIn(ids);

        var messages = await query.WithSenderOrRecipient(requiredParticipant)
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return messages;
    }

    public async Task Update(Message message)
    {
        _messages.Update(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(IEnumerable<Message> messages)
    {
        _dbContext.UpdateRange(messages);
        await _dbContext.SaveChangesAsync();
    }
}
