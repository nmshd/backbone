using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;

public class RejectDeletionProcessCommand : IRequest<RejectDeletionProcessResponse>
{
    public required string DeletionProcessId { get; init; }
}
