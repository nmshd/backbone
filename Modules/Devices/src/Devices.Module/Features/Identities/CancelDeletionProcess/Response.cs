using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;

namespace Backbone.Modules.Devices.Module.Features.Identities.CancelDeletionProcess;

public class Response : IdentityDeletionProcessOverviewDTO
{
    public Response(IdentityDeletionProcess deletionProcess) : base(deletionProcess)
    {
    }
}
