using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Commands.DeleteOrphanedMessages;

public class DeleteOrphanedMessagesCommand : IRequest
{
    public DeleteOrphanedMessagesCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
