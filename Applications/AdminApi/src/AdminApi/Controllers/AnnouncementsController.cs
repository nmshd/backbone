using Backbone.BuildingBlocks.API.Mvc;
using Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;
using Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncements;
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
    public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Created(response);
    }

    [HttpGet]
    public async Task<IActionResult> ListAnnouncements(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllAnnouncementsQuery(), cancellationToken);
        return Ok(response);
    }
}
