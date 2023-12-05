using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages.Commands.DeleteMessagesByIdentity;

public class Handler(IMessagesRepository messagesRepository, IOptions<ApplicationOptions> applicationOptions) : IRequestHandler<DeleteMessagesByIdentityCommand>
{
    private const string DELETED_IDENTITY_STRING = "deleted identity";
    private readonly IMessagesRepository _messagesRepository = messagesRepository;
    private readonly IOptions<ApplicationOptions> _applicationOptions = applicationOptions;

    public async Task Handle(DeleteMessagesByIdentityCommand request, CancellationToken cancellationToken)
    {
        var messages = await _messagesRepository.FindMessagesWithParticipant(request.IdentityAddress, cancellationToken);

        var newIdentityAddress = IdentityAddress.Create(Convert.FromBase64String(DELETED_IDENTITY_STRING), _applicationOptions.Value.AddressPrefix);

        foreach (var message in messages)
        {
            message.ReplaceIdentityAddress(request.IdentityAddress, newIdentityAddress);
        }

        await _messagesRepository.Update(messages);
    }
}
