using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;

namespace Backbone.Modules.Devices.Module.Features.Identities.CancelDeletionProcess;

public class CancelDeletionProcessResponse : IdentityDeletionProcessOverviewDTO
{
    public CancelDeletionProcessResponse(IdentityDeletionProcess deletionProcess) : base(deletionProcess)
    {
    }
}
