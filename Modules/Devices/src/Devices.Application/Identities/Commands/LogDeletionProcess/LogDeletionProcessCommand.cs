using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.LogDeletionProcess;

public class LogDeletionProcessCommand : IRequest
{
    public required string IdentityAddress { get; init; }
    public required string AggregateType { get; init; }
}
