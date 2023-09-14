using AdminUi.Infrastructure.DTOs;
using AdminUi.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
using Backbone.Modules.Devices.Application.Clients.Commands.CreateClients;
using Backbone.Modules.Devices.Application.Clients.Commands.DeleteClient;
using Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminUi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class ClientsController : ApiControllerBase
{
    private readonly AdminUiDbContext _adminUiDbContext;

    public ClientsController(IMediator mediator, AdminUiDbContext adminUiDbContext) : base(mediator)
    {
        _adminUiDbContext = adminUiDbContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ClientOverview>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllClients(CancellationToken cancellationToken)
    {
        var clientOverviews = await _adminUiDbContext.ClientOverviews.ToListAsync(cancellationToken);
        return Ok(clientOverviews);
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
