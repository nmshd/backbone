using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.HandleCompletedDeletionProcess;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
    public required IEnumerable<string> Usernames { get; init; }
}
