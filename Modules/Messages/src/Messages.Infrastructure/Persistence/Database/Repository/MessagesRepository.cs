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
    public async Task<Message> Find(MessageId id, IdentityAddress address, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _messages : _readOnlyMessages)
            .IncludeAllReferences()
            .WithSenderOrRecipient(address)
            .FirstWithId(id, cancellationToken);
    }

    public async Task<Message> FindPlain(MessageId id, CancellationToken cancellationToken)
    {
        return await _readOnlyMessages.FirstWithId(id, cancellationToken);
    }

    public async Task<MessageId> Add(Message message, CancellationToken cancellationToken)
    {
        var add = await _messages.AddAsync(message, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _blobStorage.Add(_blobOptions.RootFolder, message.Id, message.Body);
        await _blobStorage.SaveAsync();
        return add.Entity.Id;
    }

    public Task<int> CountUnreceivedMessagesFromSenderToReceiver(IdentityAddress sender, IdentityAddress recipient, CancellationToken cancellationToken)
    {
        return _readOnlyMessages
            .FromASpecificSender(sender)
            .WithASpecificRecipientWhoDidNotReceiveTheMessage(recipient)
            .CountAsync(cancellationToken);
    }

    public Task<DbPaginationResult<Message>> FindMessagesWithIds(IEnumerable<MessageId> ids, IdentityAddress requiredParticipant, PaginationFilter paginationFilter, bool track = false)
    {
        var query = (track ? _messages : _readOnlyMessages)
            .AsQueryable()
            .IncludeAllReferences();

        if (ids.Any())
            query = query.WithIdsIn(ids);

        return query.WithSenderOrRecipient(requiredParticipant)
            .DoNotSendBeforePropertyIsNotInTheFuture()
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter);
    }

    public async Task Update(Message message)
    {
        _messages.Update(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task FetchedMessage(Message message, IdentityAddress address, DeviceId deviceId)
    {
        var recipient = message.Recipients.FirstWithIdOrDefault(address);

        recipient.ReceivedMessage(deviceId);

        await Update(message);
    }
}
