using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;

public class StartDeletionProcessAsSupportResponse
{
    public StartDeletionProcessAsSupportResponse(IdentityDeletionProcess deletionProcess)
    {
        Id = deletionProcess.Id;
        Status = deletionProcess.Status;
        CreatedAt = deletionProcess.CreatedAt;
    }

    public string Id { get; set; }
    public DeletionProcessStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
