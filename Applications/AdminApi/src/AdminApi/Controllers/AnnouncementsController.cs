using System.Threading;
using Backbone.BuildingBlocks.API.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class AnnouncementsController : ApiControllerBase
{
    public AnnouncementsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CreateAnnouncement(CancellationToken cancellationToken)
    {
        return Ok();
    }
}
