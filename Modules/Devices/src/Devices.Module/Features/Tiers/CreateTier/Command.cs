using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.CreateTier;

public class CreateTierCommand : IRequest<CreateTierResponse>
{
    public required string Name { get; init; }
}
