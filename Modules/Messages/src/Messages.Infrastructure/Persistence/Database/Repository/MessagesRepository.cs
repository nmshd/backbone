using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using Enmeshed.BuildingBlocks.Application.Pagination;


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
            .IncludeAllReferences()
            .WithSenderOrRecipient(address)
            .FirstWithId(id, cancellationToken);

        if (fillBody)
        {
            await FillBody(message);
        }

        return message;
    }

    public async Task<MessageId> Add(Message message, CancellationToken cancellationToken)
    {
        var add = await _messages.AddAsync(message, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _blobStorage.Add(_blobOptions.RootFolder, message.Id, message.Body);
        await _blobStorage.SaveAsync();
        return add.Entity.Id;
    }

    public async Task<int> CountUnreceivedMessagesFromSenderToRecipient(IdentityAddress sender, IdentityAddress recipient, CancellationToken cancellationToken)
    {
        return await _readOnlyMessages
            .FromASpecificSender(sender)
            .WithASpecificRecipientWhoDidNotReceiveTheMessage(recipient)
            .CountAsync(cancellationToken);
    }

    public async Task<DbPaginationResult<Message>> FindMessagesWithIds(IEnumerable<MessageId> ids, IdentityAddress requiredParticipant, PaginationFilter paginationFilter, bool track = false)
    {
        var query = (track ? _messages : _readOnlyMessages)
            .AsQueryable()
            .IncludeAllReferences();

        if (ids.Any())
            query = query.WithIdsIn(ids);

        var messages = await query.WithSenderOrRecipient(requiredParticipant)
            .DoNotSendBeforePropertyIsNotInTheFuture()
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter);

        await Task.WhenAll(messages.ItemsOnPage.Select(FillBody).ToArray());

        return messages;
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
