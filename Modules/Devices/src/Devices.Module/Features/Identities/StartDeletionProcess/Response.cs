using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;

namespace Backbone.Modules.Devices.Module.Features.Identities.StartDeletionProcess;

public class StartDeletionProcessResponse : IdentityDeletionProcessOverviewDTO
{
    public StartDeletionProcessResponse(IdentityDeletionProcess deletionProcess) : base(deletionProcess)
    {
    }
}
