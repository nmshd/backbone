using Backbone.Modules.Devices.Module.Features.Tiers.Shared;

namespace Backbone.Modules.Devices.Module.Features.Tiers.CreateTier;

public class CreateTierResponse : TierDTO
{
    public CreateTierResponse(string id, string name) : base(id, name)
    {
    }
}
