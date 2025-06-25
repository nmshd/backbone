using MediatR;

namespace Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;

public class DeleteTierCommand : IRequest
{
    public required string TierId { get; init; }
}
