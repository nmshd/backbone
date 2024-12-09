using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.HandleCompletedDeletionProcess;

public class HandleCompletedDeletionProcessCommand : IRequest
{
    public HandleCompletedDeletionProcessCommand(string identityAddress, IEnumerable<string> usernames)
    {
        IdentityAddress = identityAddress;
        Usernames = usernames;
    }

    public string IdentityAddress { get; }
    public IEnumerable<string> Usernames { get; }
}
