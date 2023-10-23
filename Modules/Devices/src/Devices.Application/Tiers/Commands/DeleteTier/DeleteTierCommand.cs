using MediatR;

namespace Backbone.Devices.Application.Tiers.Commands.DeleteTier;

public class DeleteTierCommand : IRequest
{
    public DeleteTierCommand(string tierId)
    {
        TierId = tierId;
    }

    public string TierId { get; set; }
}
