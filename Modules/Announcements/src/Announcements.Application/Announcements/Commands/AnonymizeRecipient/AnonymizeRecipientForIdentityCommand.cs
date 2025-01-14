using MediatR;

namespace Backbone.Modules.Announcements.Application.Announcements.Commands.AnonymizeRecipient;

public record AnonymizeRecipientForIdentityCommand : IRequest
{
    public AnonymizeRecipientForIdentityCommand(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; }
}
