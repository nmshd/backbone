using CreateAnnouncement = Backbone.Modules.Announcements.Module.Features.Announcements.CreateAnnouncement;
using DeleteAnnouncementById = Backbone.Modules.Announcements.Module.Features.Announcements.DeleteAnnouncementById;
using GetAnnouncementById = Backbone.Modules.Announcements.Module.Features.Announcements.GetAnnouncementById;
using ListAnnouncements = Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncements;
using Backbone.AdminApi.Versions;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Announcements.Module.Features.Announcements.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.AdminApi.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("ApiKey")]
[V1]
public class AnnouncementsController : ApiControllerBase
{
    public AnnouncementsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(AnnouncementDTO), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncement.Command request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Created(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAnnouncement(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteAnnouncementById.Command { Id = id }, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AnnouncementDTO), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnnouncement(string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAnnouncementById.Query { Id = id }, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListAnnouncements.Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAnnouncements(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListAnnouncements.Query(), cancellationToken);
        return Ok(response);
    }
}
