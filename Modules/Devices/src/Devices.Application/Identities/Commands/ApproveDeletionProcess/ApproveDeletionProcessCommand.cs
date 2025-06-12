using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;

public class ApproveDeletionProcessCommand : IRequest<ApproveDeletionProcessResponse>
{
    public required string DeletionProcessId { get; init; }
}
