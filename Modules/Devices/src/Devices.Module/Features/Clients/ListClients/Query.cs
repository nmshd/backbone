using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.ListClients;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListClientsQuery")]
public class Query : IRequest<Response>;
