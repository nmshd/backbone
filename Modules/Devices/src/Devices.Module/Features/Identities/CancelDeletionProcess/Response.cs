using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;

namespace Backbone.Modules.Devices.Module.Features.Identities.CancelDeletionProcess;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CancelDeletionProcessResponse")]
public class Response : IdentityDeletionProcessOverviewDTO
{
    public Response(IdentityDeletionProcess deletionProcess) : base(deletionProcess)
    {
    }
}
