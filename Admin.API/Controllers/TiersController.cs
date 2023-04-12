using Microsoft.AspNetCore.Mvc;
using MediatR;
using Admin.API.Mvc;
using Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;

namespace Admin.API.Controllers;

[Route("api/v1/[controller]")]
public class TiersController : ApiControllerBase
{
    public TiersController(IMediator mediator) : base(mediator) { }

    [HttpPost]
    [ProducesResponseType(typeof(CreateTierResponse), StatusCodes.Status201Created)]
    public async Task<CreatedResult> PostTiers([FromBody] CreateTierCommand command)
    {
        var createdTier = await _mediator.Send(command);
        return Created(createdTier);
    }
}
