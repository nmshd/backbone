using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.ConsumerApi.Versions;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcess;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcess;
using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsOwner;
using Backbone.Modules.Devices.Application.Identities.Queries.ListDeletionProcessesAsOwner;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.ConsumerApi.Controllers.Devices;

[V2]
[Route("api/v{v:apiVersion}/Identities/Self/DeletionProcesses")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class IdentityDeletionProcessesController : ApiControllerBase
{
    public IdentityDeletionProcessesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<StartDeletionProcessResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartDeletionProcess(StartDeletionProcessCommand? request, CancellationToken cancellationToken)
    {
        request ??= new StartDeletionProcessCommand();
        var response = await _mediator.Send(request, cancellationToken);

        return Created("", response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<IdentityDeletionProcessOverviewDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeletionProcess([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetDeletionProcessAsOwnerQuery { Id = id }, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ListDeletionProcessesAsOwnerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListDeletionProcesses(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListDeletionProcessesAsOwnerQuery(), cancellationToken);

        return Ok(response);
    }

    [HttpPut("{id}/Cancel")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CancelDeletionProcessResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelDeletionProcess([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CancelDeletionProcessCommand { DeletionProcessId = id }, cancellationToken);

        return Ok(response);
    }
}
