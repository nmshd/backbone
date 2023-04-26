using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages;

public class MessageService
{
    private readonly IBlobStorage _blobStorage;
    private readonly IDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<MessageService> _logger;
    private readonly IUserContext _userContext;
    private readonly BlobOptions _blobOptions;

    public MessageService(IEventBus eventBus, IUserContext userContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, ILogger<MessageService> logger)
    {
        _eventBus = eventBus;
        _userContext = userContext;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
        _logger = logger;
    }

    public async Task MarkMessageAsReceived(Message message, CancellationToken cancellationToken)
    {
        var wasSet = MarkMessageAsReceived(message);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (wasSet)
            _eventBus.Publish(new MessageDeliveredIntegrationEvent(message, _userContext.GetAddress()));
    }

    public async Task MarkMessagesAsReceived(IEnumerable<Message> messages, CancellationToken cancellationToken)
    {
        var messagesWithSetReceptionDate = new List<Message>();

        foreach (var message in messages)
        {
            var wasSet = MarkMessageAsReceived(message);

            if (wasSet)
                messagesWithSetReceptionDate.Add(message);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogTrace($"Marked {messagesWithSetReceptionDate.Count} messages as received.");

        foreach (var message in messagesWithSetReceptionDate)
        {
            _eventBus.Publish(new MessageDeliveredIntegrationEvent(message, _userContext.GetAddress()));
        }
    }

    private bool MarkMessageAsReceived(Message message)
    {
        var recipient = message.Recipients.FirstWithIdOrDefault(_userContext.GetAddress());

        if (recipient == null || recipient.ReceivedAt.HasValue)
            return false;

        recipient.ReceivedMessage(_userContext.GetDeviceId());
        _dbContext.Set<Message>().Update(message);

        _logger.LogTrace($"Marked message with id {message.Id} as received for recipient {recipient.Address}.");

        return true;
    }

    public async Task FillBody(MessageDTO dto)
    {
        dto.Body = await _blobStorage.FindAsync(_blobOptions.RootFolder, dto.Id);
    }

    public async Task FillBodies(IEnumerable<MessageDTO> dtos)
    {
        var fillBodyTasks = dtos.Select(FillBody);
        await Task.WhenAll(fillBodyTasks);
    }
}
