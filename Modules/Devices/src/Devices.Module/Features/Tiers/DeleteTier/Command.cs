using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.DeleteTier;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteTierCommand")]
public class Command : IRequest
{
    public required string TierId { get; init; }
}
