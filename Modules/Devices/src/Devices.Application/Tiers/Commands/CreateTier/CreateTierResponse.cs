using Backbone.Modules.Devices.Application.Tiers.DTOs;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;

public class CreateTierResponse : TierDTO
{
    public CreateTierResponse(string id, string name) : base(id, name)
    {
    }
}
