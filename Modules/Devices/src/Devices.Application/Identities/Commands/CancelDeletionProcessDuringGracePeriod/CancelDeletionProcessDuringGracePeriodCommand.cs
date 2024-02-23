using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessDuringGracePeriod;

public class CancelDeletionProcessDuringGracePeriodCommand : IRequest<CancelDeletionProcessDuringGracePeriodResponse>
{
    public CancelDeletionProcessDuringGracePeriodCommand(string id)
    {
        DeletionProcessId = id;
    }

    public string DeletionProcessId { get; set; }
}
