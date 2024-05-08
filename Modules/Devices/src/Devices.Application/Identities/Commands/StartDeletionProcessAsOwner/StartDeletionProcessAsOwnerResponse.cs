using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsOwner;

public class StartDeletionProcessAsOwnerResponse : IdentityDeletionProcessOverviewDTO
{
    public StartDeletionProcessAsOwnerResponse(IdentityDeletionProcess deletionProcess) : base(deletionProcess)
    {
    }
}
