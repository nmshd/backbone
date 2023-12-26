using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Commands.DeleteMessagesOfIdentity;

// TODO this command does not actually delete the messages, it changes the author/recipients to a dummy one.
// it might be interesting to add a backlog task to delete a message when all related Identities have been deleted.
public class DeleteMessagesOfIdentityCommand(IdentityAddress identityAddress) : IRequest
{
    public IdentityAddress IdentityAddress { get; set; } = identityAddress;
}
