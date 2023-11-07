using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Infrastructure.Persistence.Database.Repository;
public class MessagesRepository : IMessagesRepository
{
    private readonly DbSet<Message> _messages;
    private readonly IQueryable<Message> _readOnlyMessages;
    private readonly MessagesDbContext _dbContext;
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;

    public MessagesRepository(MessagesDbContext dbContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions)
    {
        _messages = dbContext.Messages;
        _readOnlyMessages = dbContext.Messages.AsNoTracking();
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
    }

    public async Task<Message> Find(MessageId id, IdentityAddress address, CancellationToken cancellationToken, bool track = false, bool fillBody = true)
    {
        var message = await (track ? _messages : _readOnlyMessages)
            .IncludeAll(_dbContext)
            .WithSenderOrRecipient(address)
            .FirstWithId(id, cancellationToken);

        await FillBody(new DbPaginationResult<Message>(new List<Message>() { message }, 1));

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

    public async Task<DbPaginationResult<Message>> FindMessagesWithIds(IEnumerable<MessageId> ids, IdentityAddress requiredParticipant, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool track = false)
    {
        var query = (track ? _messages : _readOnlyMessages)
            .AsQueryable()
            .IncludeAll(_dbContext);

        if (ids.Any())
            query = query.WithIdsIn(ids);

        var messages = await query.WithSenderOrRecipient(requiredParticipant)
            .DoNotSendBeforePropertyIsNotInTheFuture()
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        await FillBody(messages);

        return messages;
    }

    private async Task FillBody(DbPaginationResult<Message> messages)
    {
        var tasks = messages.ItemsOnPage
            .Where(message => message.Body.Length == 0)
            .Select(FillBody)
            .ToList();

        await Task.WhenAll(tasks);
    }

    private async Task FillBody(Message message)
    {
        message.LoadBody(await _blobStorage.FindAsync(_blobOptions.RootFolder, message.Id));
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
