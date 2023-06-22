using Backbone.Modules.Devices.Application.Clients.DTOs;
using Enmeshed.BuildingBlocks.Application.CQRS.BaseClasses;

namespace Backbone.Modules.Devices.Application.Clients.Commands.CreateClients;
public class CreateClientResponse : ClientDTO
{
    public CreateClientResponse(string clientId, string displayName) : base(clientId, displayName)
    {
    }
}