using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcess;

public class CancelDeletionProcessCommand : IRequest<CancelDeletionProcessResponse>
{
    public CancelDeletionProcessCommand(string id)
    {
        DeletionProcessId = id;
    }

    public string DeletionProcessId { get; set; }
}
