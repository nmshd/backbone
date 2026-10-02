using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.DeleteTier;

public class Command : IRequest
{
    public required string TierId { get; init; }
}
