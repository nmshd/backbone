using MediatR;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;

public class CreateTierCommand : IRequest<CreateTierResponse>
{
    public string Name { get; set; }
}
