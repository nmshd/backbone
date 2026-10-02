using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.CreateTier;

public class Command : IRequest<Response>
{
    public required string Name { get; init; }
}
