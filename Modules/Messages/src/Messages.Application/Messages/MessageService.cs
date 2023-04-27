using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages;

public class MessageService
{
    private readonly IBlobStorage _blobStorage;
    private readonly IEventBus _eventBus;
    private readonly ILogger<MessageService> _logger;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IUserContext _userContext;
    private readonly BlobOptions _blobOptions;

    public MessageService(IEventBus eventBus, IUserContext userContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, ILogger<MessageService> logger, IMessagesRepository messagesRepository)
    {
        _eventBus = eventBus;
        _userContext = userContext;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
        _logger = logger;
        _messagesRepository = messagesRepository;
    }

    public async Task MarkMessagesAsReceived(IEnumerable<Message> messages)
    {
        await Task.WhenAll(messages.Select(MarkMessageAsReceived));
    }

    public Task MarkMessageAsReceived(Message message)
    {
        _messagesRepository.FetchedMessage(message, _userContext.GetAddress(), _userContext.GetDeviceId());

        return Task.CompletedTask;
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
