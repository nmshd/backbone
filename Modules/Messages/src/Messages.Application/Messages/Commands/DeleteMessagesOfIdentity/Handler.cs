using System.Text;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages.Commands.DeleteMessagesOfIdentity;

public class Handler(IMessagesRepository messagesRepository, IOptions<ApplicationOptions> applicationOptions) : IRequestHandler<DeleteMessagesOfIdentityCommand>
{
    private const string DELETED_IDENTITY_STRING = "deleted identity";

    public async Task Handle(DeleteMessagesOfIdentityCommand request, CancellationToken cancellationToken)
    {
        var messages = await messagesRepository.FindMessagesWithParticipant(request.IdentityAddress, cancellationToken);

        var newIdentityAddress = IdentityAddress.Create(Encoding.Unicode.GetBytes(DELETED_IDENTITY_STRING), applicationOptions.Value.AddressPrefix);

        foreach (var message in messages)
        {
            message.ReplaceIdentityAddress(request.IdentityAddress, newIdentityAddress);
        }

        await messagesRepository.Update(messages);
    }
}
