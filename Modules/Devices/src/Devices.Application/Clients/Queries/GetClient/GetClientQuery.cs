using Backbone.Modules.Devices.Application.Clients.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.GetClient;

public class GetClientQuery : IRequest<ClientDTO>
{
    public required string Id { get; set; }
}
