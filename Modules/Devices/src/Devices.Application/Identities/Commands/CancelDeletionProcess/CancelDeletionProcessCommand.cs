using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcess;

public class CancelDeletionProcessCommand : IRequest<CancelDeletionProcessResponse>
{
    public required string DeletionProcessId { get; init; }
}
