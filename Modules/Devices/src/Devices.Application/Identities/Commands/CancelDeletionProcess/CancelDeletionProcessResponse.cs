using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcess;

public class CancelDeletionProcessResponse : IdentityDeletionProcessOverviewDTO
{
    public CancelDeletionProcessResponse(IdentityDeletionProcess deletionProcess) : base(deletionProcess)
    {
    }
}
