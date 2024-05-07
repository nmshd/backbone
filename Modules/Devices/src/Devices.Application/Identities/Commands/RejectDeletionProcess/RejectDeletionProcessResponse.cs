using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;

public class RejectDeletionProcessResponse : IdentityDeletionProcessOverviewDTO
{
    public RejectDeletionProcessResponse(IdentityDeletionProcess deletionProcess) : base(deletionProcess)
    {
    }
}
