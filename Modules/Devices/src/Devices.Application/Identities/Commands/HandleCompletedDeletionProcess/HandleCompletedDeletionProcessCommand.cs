using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.HandleCompletedDeletionProcess;

public class HandleCompletedDeletionProcessCommand : IRequest
{
    public HandleCompletedDeletionProcessCommand(IdentityAddress identityAddress, IEnumerable<string> usernames)
    {
        IdentityAddress = identityAddress;
        Usernames = usernames;
    }

    public IdentityAddress IdentityAddress { get; }
    public IEnumerable<string> Usernames { get; }
}
