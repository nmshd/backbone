using Backbone.AdminApi.Versions;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Challenges.Application.Challenges.Commands.CreateChallenge;
using Backbone.Modules.Challenges.Application.Challenges.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminApi.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("ApiKey")]
[V1]
public class ChallengesController : ApiControllerBase
{
    public ChallengesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ChallengeDTO>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateChallengeCommand(), cancellationToken);
        return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
    }
}
