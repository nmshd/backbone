using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Identities.Commands.DeleteIdentity;

public class Handler : IRequestHandler<DeleteIdentityCommand>
{
    private const string DELETED_IDENTITY_STRING = "deleted identity";
    private readonly IMessagesRepository _messagesRepository;
    private readonly IOptions<ApplicationOptions> _applicationOptions;

    public Handler(IMessagesRepository messagesRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _messagesRepository = messagesRepository;
        _applicationOptions = applicationOptions;
    }

    public async Task Handle(DeleteIdentityCommand request, CancellationToken cancellationToken)
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
