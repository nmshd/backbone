using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Commands.AnonymizeMessagesOfIdentity;

public class AnonymizeMessagesOfIdentityCommand : IRequest
{
    public AnonymizeMessagesOfIdentityCommand(IdentityAddress identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public IdentityAddress IdentityAddress { get; set; }
}
