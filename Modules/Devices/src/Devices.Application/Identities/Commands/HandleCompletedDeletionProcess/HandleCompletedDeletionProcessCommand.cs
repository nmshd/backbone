using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.HandleCompletedDeletionProcess;

public class HandleCompletedDeletionProcessCommand : IRequest
{
    public required string IdentityAddress { get; init; }
    public required IEnumerable<string> Usernames { get; init; }
}
