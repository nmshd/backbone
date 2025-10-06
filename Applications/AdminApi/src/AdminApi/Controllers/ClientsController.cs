using Backbone.AdminApi.DTOs;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.AdminApi.Versions;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
using Backbone.Modules.Devices.Application.Clients.Commands.CreateClient;
using Backbone.Modules.Devices.Application.Clients.Commands.DeleteClient;
using Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;
using Backbone.Modules.Devices.Application.Clients.DTOs;
using Backbone.Modules.Devices.Application.Clients.Queries.GetClient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backbone.AdminApi.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("ApiKey")]
[V1]
public class ClientsController : ApiControllerBase
{
    private readonly AdminApiDbContext _adminApiDbContext;

    public ClientsController(IMediator mediator, AdminApiDbContext adminApiDbContext) : base(mediator)
    {
        _adminApiDbContext = adminApiDbContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ClientOverviewDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListClients(CancellationToken cancellationToken)
    {
        var clientOverviews = await _adminApiDbContext.OpenIddictApplications.Select(a => new ClientOverviewDTO
        {
            CreatedAt = a.CreatedAt,
            ClientId = a.ClientId,
            DefaultTier = _adminApiDbContext.Tiers.Select(t => new TierDTO { Id = t.Id, Name = t.Name }).First(t => t.Id == a.DefaultTier),
            DisplayName = a.DisplayName,
            NumberOfIdentities = _adminApiDbContext.Identities.Count(i => i.ClientId == a.ClientId),
            MaxIdentities = a.MaxIdentities
        }).ToListAsync(cancellationToken);
        return Ok(clientOverviews);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ClientDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClient([FromRoute] string id, CancellationToken cancellationToken)
    {
        var client = await _mediator.Send(new GetClientQuery { Id = id }, cancellationToken);
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
        var changedClient = await _mediator.Send(new ChangeClientSecretCommand { ClientId = clientId, NewSecret = request.NewSecret }, cancellationToken);
        return Ok(changedClient);
    }

    [HttpPut("{clientId}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<UpdateClientResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateClient([FromRoute] string clientId, [FromBody] UpdateClientRequest request, CancellationToken cancellationToken)
    {
        var updatedClient = await _mediator.Send(new UpdateClientCommand { ClientId = clientId, DefaultTier = request.DefaultTier, MaxIdentities = request.MaxIdentities }, cancellationToken);
        return Ok(updatedClient);
    }

    [HttpDelete("{clientId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteClient([FromRoute] string clientId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteClientCommand { ClientId = clientId }, cancellationToken);
        return NoContent();
    }
}

public class ChangeClientSecretRequest
{
    public required string NewSecret { get; set; }
}

public class UpdateClientRequest
{
    public required string DefaultTier { get; set; }
    public int? MaxIdentities { get; set; }
}
