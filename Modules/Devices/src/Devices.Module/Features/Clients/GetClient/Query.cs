using Backbone.Modules.Devices.Module.Features.Clients.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.GetClient;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetClientQuery")]
public class Query : IRequest<ClientDTO>
{
    public required string Id { get; set; }
}
