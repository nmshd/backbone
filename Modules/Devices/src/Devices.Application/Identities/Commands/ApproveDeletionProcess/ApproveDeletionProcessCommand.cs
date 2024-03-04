using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;

public class ApproveDeletionProcessCommand : IRequest<ApproveDeletionProcessResponse>
{
    public ApproveDeletionProcessCommand(string deletionProcessId)
    {
        DeletionProcessId = deletionProcessId;
    }

    public string DeletionProcessId { get; set; }
}
