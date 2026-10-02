using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.CancelDeletionProcess;

public class Command : IRequest<Response>
{
    public required string DeletionProcessId { get; init; }
}
