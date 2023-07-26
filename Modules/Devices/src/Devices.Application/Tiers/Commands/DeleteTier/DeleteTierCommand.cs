using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using MediatR;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;

public class DeleteTierCommand : IRequest<DeleteTierResponse>
{
    public DeleteTierCommand(string tierId)
    {
        TierId = TierId.Create(tierId).Value;
    }

    public TierId TierId { get; set; }
}
