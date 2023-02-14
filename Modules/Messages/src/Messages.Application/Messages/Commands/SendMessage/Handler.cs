using AutoMapper;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;

public class Handler : IRequestHandler<SendMessageCommand, SendMessageResponse>
{
    private readonly IBlobStorage _blobStorage;
    private readonly IMessagesDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<Handler> _logger;
    private readonly IMapper _mapper;
    private readonly ApplicationOptions _options;
    private readonly IUserContext _userContext;
    private readonly BlobOptions _blobOptions;

    public Handler(IMessagesDbContext dbContext, IBlobStorage blobStorage, IUserContext userContext, IMapper mapper, IEventBus eventBus, IOptionsSnapshot<ApplicationOptions> options, ILogger<Handler> logger, IOptions<BlobOptions> blobOptions)
    {
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _userContext = userContext;
        _mapper = mapper;
        _eventBus = eventBus;
        _logger = logger;
        _options = options.Value;
        _blobOptions = blobOptions.Value;
    }

    public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var recipients = await ValidateRecipients(request);

        var message = new Message(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.DoNotSendBefore,
            request.Body,
            request.Attachments.Select(a => new Attachment(FileId.Parse(a.Id))),
            recipients);

        await SaveMessage(message, cancellationToken);

        _eventBus.Publish(new MessageCreatedIntegrationEvent(message));

        return _mapper.Map<SendMessageResponse>(message);
    }

    private async Task SaveMessage(Message message, CancellationToken cancellationToken)
    {
        await _dbContext.Set<Message>().AddAsync(message, cancellationToken);
        _blobStorage.Add(_blobOptions.RootFolder, message.Id, message.Body);

        await _blobStorage.SaveAsync();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<RecipientInformation>> ValidateRecipients(SendMessageCommand request)
    {
        _logger.LogTrace("Validating recipients...");

        var sender = _userContext.GetAddress();
        var recipients = new List<RecipientInformation>();

        foreach (var recipientDto in request.Recipients)
        {
            var idOfRelationshipBetweenSenderAndRecipient = await _dbContext
                .SetReadOnly<Relationship>()
                .WithParticipants(sender, recipientDto.Address)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (idOfRelationshipBetweenSenderAndRecipient == null)
            {
                _logger.LogInformation($"Sending message aborted. There is no relationship between sender ({sender}) and recipient ({recipientDto.Address}).");
                throw new OperationFailedException(ApplicationErrors.NoRelationshipToRecipientExists(recipientDto.Address));
            }

            var numberOfUnreceivedMessagesFromActiveIdentity = await _dbContext
                .SetReadOnly<Message>()
                .FromASpecificSender(sender)
                .WithASpecificRecipientWhoDidNotReceiveTheMessage(recipientDto.Address)
                .CountAsync();

            if (numberOfUnreceivedMessagesFromActiveIdentity >= _options.MaxNumberOfUnreceivedMessagesFromOneSender)
            {
                _logger.LogInformation($"Sending message aborted. Recipient {recipientDto.Address} already has {numberOfUnreceivedMessagesFromActiveIdentity} unreceived messages from sender {sender}, which is more than the maximum ({_options.MaxNumberOfUnreceivedMessagesFromOneSender}).");
                throw new OperationFailedException(ApplicationErrors.MaxNumberOfUnreceivedMessagesReached(recipientDto.Address));
            }

            var recipient = new RecipientInformation(recipientDto.Address, idOfRelationshipBetweenSenderAndRecipient, recipientDto.EncryptedKey);

            recipients.Add(recipient);
        }

        _logger.LogInformation("Successfully validated all recipients.");

        return recipients;
    }
}
