using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcess;

public class CancelDeletionProcessCommand : IRequest<CancelDeletionProcessResponse>
{
    public CancelDeletionProcessCommand(string deletionProcessId)
    {
        DeletionProcessId = deletionProcessId;
    }

    public string DeletionProcessId { get; set; }
}
