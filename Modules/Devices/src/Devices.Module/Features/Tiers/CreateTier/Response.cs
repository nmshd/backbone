using Backbone.Modules.Devices.Module.Features.Tiers.Shared;

namespace Backbone.Modules.Devices.Module.Features.Tiers.CreateTier;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateTierResponse")]
public class Response : TierDTO
{
    public Response(string id, string name) : base(id, name)
    {
    }
}
