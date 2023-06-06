using Backbone.Modules.Devices.Application.Clients.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
public class ListClientsQuery : IRequest<IEnumerable<ClientDTO>> { }
