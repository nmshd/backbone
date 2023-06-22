﻿using AdminApi.Mvc;
using Backbone.Modules.Devices.Application.Clients.Commands.CreateClients;
using Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
using Enmeshed.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminApi.Controllers;

[Route("api/v1/[controller]")]

public class ClientsController : ApiControllerBase
{
    public ClientsController(IMediator mediator) : base(mediator) { }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ListClientsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllClients(CancellationToken cancellationToken)
    {
        var clients = await _mediator.Send(new ListClientsQuery(), cancellationToken);
        return Ok(clients);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateClientResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<CreatedResult> CreateOAuthClients(CreateClientCommand command, CancellationToken cancellationToken)
    {
        var createdClient = await _mediator.Send(new CreateClientCommand(command.ClientId, command.DisplayName, command.ClientSecret), cancellationToken);
        return Created(createdClient);
    }   
}
