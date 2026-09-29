using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Messages.Abstractions.Persistence;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Module.Features.Messages.SendMessage;

public class Handler : IRequestHandler<SendMessageCommand, SendMessageResponse>
{
    private readonly ILogger<Handler> _logger;
    private readonly ApplicationConfiguration _configuration;
    private readonly IUserContext _userContext;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IRelationshipsRepository _relationshipsRepository;

    public Handler(IUserContext userContext, IOptionsSnapshot<ApplicationConfiguration> options, ILogger<Handler> logger,
        IMessagesRepository messagesRepository, IRelationshipsRepository relationshipsRepository)
    {
        _userContext = userContext;
        _logger = logger;
        _configuration = options.Value;
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
            request.Attachments.Select(attachment => new Attachment(FileId.Parse(attachment.Id))),
            recipients);

        await _messagesRepository.Add(message, cancellationToken);
        return new SendMessageResponse(message);
    }

    private async Task<List<RecipientInformation>> ValidateRecipients(SendMessageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Validating recipients...");
        var sender = _userContext.GetAddress();
        var recipients = new List<RecipientInformation>();

        var index = 0;
        foreach (var recipientDto in request.Recipients)
        {
            var relationship = await _relationshipsRepository.GetYoungestRelationship(sender, recipientDto.Address, cancellationToken);
            if (relationship == null)
            {
                _logger.LogInformation("Sending message aborted. There is no relationship between the sender and the recipient at index {recipientIndex}.", index);
                throw new OperationFailedException(ApplicationErrors.NoRelationshipToRecipientExists(recipientDto.Address));
            }

            var numberOfUnreceivedMessages = await _messagesRepository.CountUnreceivedMessagesFromSenderToRecipient(sender, recipientDto.Address, cancellationToken);
            relationship.EnsureSendingMessagesIsAllowed(sender, numberOfUnreceivedMessages, _configuration.MaxNumberOfUnreceivedMessagesFromOneSender);
            recipients.Add(new RecipientInformation(recipientDto.Address, relationship.Id, recipientDto.EncryptedKey));
            index++;
        }

        _logger.LogInformation("Successfully validated all recipients.");
        return recipients;
    }
}
