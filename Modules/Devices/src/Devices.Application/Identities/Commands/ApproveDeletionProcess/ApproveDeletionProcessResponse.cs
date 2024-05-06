using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;

public class ApproveDeletionProcessResponse : IdentityDeletionProcessOverviewDTO
{
    public ApproveDeletionProcessResponse(IdentityDeletionProcess deletionProcess) : base(deletionProcess)
    {
    }
}
