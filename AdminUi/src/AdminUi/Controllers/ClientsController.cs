﻿using Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
using Backbone.Modules.Devices.Application.Clients.Commands.CreateClient;
using Backbone.Modules.Devices.Application.Clients.Commands.DeleteClient;
using Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;
using Backbone.Modules.Devices.Application.Clients.DTOs;
using Backbone.Modules.Devices.Application.Clients.Queries.GetClient;
using Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminUi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
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

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClientDTO), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClient([FromRoute] string id, CancellationToken cancellationToken)
    {
        var client = await _mediator.Send(new GetClientQuery(id), cancellationToken);
        return Ok(client);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateClientResponse>), StatusCodes.Status200OK)]
    public async Task<CreatedResult> CreateOAuthClients(CreateClientCommand command, CancellationToken cancellationToken)
    {
        var createdClient = await _mediator.Send(command, cancellationToken);
        return Created(createdClient);
    }

    [HttpPatch("{clientId}/ChangeSecret")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ChangeClientSecretResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeClientSecret([FromRoute] string clientId, [FromBody] ChangeClientSecretRequest request, CancellationToken cancellationToken)
    {
        var changedClient = await _mediator.Send(new ChangeClientSecretCommand(clientId, request.NewSecret), cancellationToken);
        return Ok(changedClient);
    }

    [HttpPatch("{clientId}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<UpdateClientResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateClient([FromRoute] string clientId, [FromBody] UpdateClientRequest request, CancellationToken cancellationToken)
    {
        var updatedClient = await _mediator.Send(new UpdateClientCommand(clientId, request.DefaultTier), cancellationToken);
        return Ok(updatedClient);
    }

    [HttpDelete("{clientId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClient([FromRoute] string clientId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteClientCommand(clientId), cancellationToken);
        return NoContent();
    }
}

public class ChangeClientSecretRequest
{
    public string NewSecret { get; set; }
}

public class UpdateClientRequest
{
    public string DefaultTier { get; set; }
}
