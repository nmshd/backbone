using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Messages.Application.Extensions;
using Messages.Application.IntegrationEvents.Outgoing;
using Messages.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messages.Application.Messages.Commands.SendMessage;

public class Handler : IRequestHandler<SendMessageCommand, SendMessageResponse>
{
    private readonly IBlobStorage _blobStorage;
    private readonly IDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<Handler> _logger;
    private readonly IMapper _mapper;
    private readonly ApplicationOptions _options;
    private readonly IUserContext _userContext;

    public Handler(IDbContext dbContext, IBlobStorage blobStorage, IUserContext userContext, IMapper mapper, IEventBus eventBus, IOptionsMonitor<ApplicationOptions> options, ILogger<Handler> logger)
    {
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _userContext = userContext;
        _mapper = mapper;
        _eventBus = eventBus;
        _logger = logger;
        _options = options.CurrentValue;
    }

    public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var recipients = await ValidateRecipients(request);

        var message = new Message(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.DoNotSendBefore,
            request.Body,
            request.Attachments.Select(a => new Attachment(a.Id)),
            recipients);

        await SaveMessage(message, cancellationToken);

        _eventBus.Publish(new MessageCreatedIntegrationEvent(message));

        return _mapper.Map<SendMessageResponse>(message);
    }

    private async Task SaveMessage(Message message, CancellationToken cancellationToken)
    {
        await _dbContext.Set<Message>().AddAsync(message, cancellationToken);
        _blobStorage.Add(message.Id, message.Body);

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
