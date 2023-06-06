using AdminApi.Mvc;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Clients.DTOs;
using Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
using Enmeshed.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AdminApi.Controllers;

[Route("api/v1/[controller]")]

public class ClientsController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public ClientsController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ClientDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllClients(CancellationToken cancellationToken)
    {
        var clients = await _mediator.Send(new ListClientsQuery(), cancellationToken);
        return Ok(clients);
    }
}
