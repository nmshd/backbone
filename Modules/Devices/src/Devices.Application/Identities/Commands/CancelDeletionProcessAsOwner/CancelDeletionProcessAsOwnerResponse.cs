using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsOwner;

public class CancelDeletionProcessAsOwnerResponse : IdentityDeletionProcessOverviewDTO
{
    public CancelDeletionProcessAsOwnerResponse(IdentityDeletionProcess deletionProcess) : base(deletionProcess)
    {
    }
}
