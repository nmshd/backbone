using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.CreateClient;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateClientCommand")]
public class Command : IRequest<Response>
{
    public string? ClientId { get; init; }

    public string? DisplayName { get; init; }

    public string? ClientSecret { get; init; }

    public required string DefaultTier { get; init; }

    public int? MaxIdentities { get; init; }
}
