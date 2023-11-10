﻿using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Devices.Application.Clients.DTOs;

namespace Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
public class ListClientsResponse : EnumerableResponseBase<ClientDTO>
{
    public ListClientsResponse(IEnumerable<ClientDTO> items) : base(items) { }
}
