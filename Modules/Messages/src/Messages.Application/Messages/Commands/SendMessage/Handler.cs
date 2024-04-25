using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages.Commands.SendMessage;

public class Handler : IRequestHandler<SendMessageCommand, SendMessageResponse>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<Handler> _logger;
    private readonly IMapper _mapper;
    private readonly ApplicationOptions _options;
    private readonly IUserContext _userContext;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(
        IUserContext userContext,
        IMapper mapper,
        IEventBus eventBus,
        IOptionsSnapshot<ApplicationOptions> options,
        ILogger<Handler> logger,
        IMessagesRepository messagesRepository,
        IRelationshipsRepository relationshipsRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _eventBus = eventBus;
        _logger = logger;
        _options = options.Value;
        _messagesRepository = messagesRepository;
        _relationshipsRepository = relationshipsRepository;
    }

    public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var recipients = await ValidateRecipients(request, cancellationToken);

        var message = new Message(
            _userContext.GetAddress(),
            _userContext.GetDeviceId(),
            request.Body,
            request.Attachments.Select(a => new Attachment(FileId.Parse(a.Id))),
            recipients);

        await _messagesRepository.Add(message, cancellationToken);

        _eventBus.Publish(new MessageCreatedDomainEvent(message));

        return _mapper.Map<SendMessageResponse>(message);
    }

    private async Task<List<RecipientInformation>> ValidateRecipients(SendMessageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Validating recipients...");

        var sender = _userContext.GetAddress();
        var recipients = new List<RecipientInformation>();

        foreach (var recipientDto in request.Recipients)
        {
            var relationshipBetweenSenderAndRecipient = await _relationshipsRepository.FindYoungestRelationship(sender, recipientDto.Address, cancellationToken);

            if (relationshipBetweenSenderAndRecipient == null)
            {
                _logger.LogInformation("Sending message aborted. There is no relationship between sender ({sender}) and recipient ({recipient}).", sender, recipientDto.Address);
                throw new OperationFailedException(ApplicationErrors.NoRelationshipToRecipientExists(recipientDto.Address));
            }

            var numberOfUnreceivedMessagesFromActiveIdentity = await _messagesRepository.CountUnreceivedMessagesFromSenderToRecipient(sender, recipientDto.Address, cancellationToken);

            relationshipBetweenSenderAndRecipient.EnsureSendingMessagesIsAllowed(
                numberOfUnreceivedMessagesFromActiveIdentity,
                _options.MaxNumberOfUnreceivedMessagesFromOneSender);

            var recipient = new RecipientInformation(recipientDto.Address, relationshipBetweenSenderAndRecipient.Id, recipientDto.EncryptedKey);

            recipients.Add(recipient);
        }

        _logger.LogInformation("Successfully validated all recipients.");

        return recipients;
    }
}
