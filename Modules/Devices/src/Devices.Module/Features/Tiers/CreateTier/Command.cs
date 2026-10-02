using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Tiers.CreateTier;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateTierCommand")]
public class Command : IRequest<Response>
{
    public required string Name { get; init; }
}
