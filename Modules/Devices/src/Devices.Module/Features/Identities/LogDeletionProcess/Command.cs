using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.LogDeletionProcess;

public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
    public required string AggregateType { get; init; }
}
