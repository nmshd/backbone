using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Announcements.Application.Announcements.Commands.CreateAnnouncement;
using Backbone.Modules.Announcements.Application.Announcements.DTOs;
using Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncements;
using Backbone.Modules.Announcements.Application.Announcements.Queries.GetAnnouncementById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class AnnouncementsController : ApiControllerBase
{
    private readonly ILogger<AnnouncementsController> _logger;

    public AnnouncementsController(IMediator mediator, ILogger<AnnouncementsController> logger) : base(mediator)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Created(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AnnouncementDTO), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnnouncement(string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAnnouncementByIdQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetAllAnnouncementsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAnnouncements(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllAnnouncementsQuery(), cancellationToken);
        return Ok(response);
    }
}
