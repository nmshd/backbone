using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.CancelDeletionProcess;

public class CancelDeletionProcessCommand : IRequest<CancelDeletionProcessResponse>
{
    public required string DeletionProcessId { get; init; }
}
