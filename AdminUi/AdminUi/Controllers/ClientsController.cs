using Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdminUi.Controllers;

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
}
