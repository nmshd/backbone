using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.DeleteClient;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteClientCommand")]
public class Command : IRequest
{
    public required string ClientId { get; init; }
}
