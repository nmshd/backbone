using MediatR;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;

public class CreateTierCommand : IRequest<CreateTierResponse>
{
    public CreateTierCommand(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}
