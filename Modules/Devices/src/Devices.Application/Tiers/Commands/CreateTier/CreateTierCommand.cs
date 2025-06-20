using MediatR;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;

public class CreateTierCommand : IRequest<CreateTierResponse>
{
    public required string Name { get; init; }
}
