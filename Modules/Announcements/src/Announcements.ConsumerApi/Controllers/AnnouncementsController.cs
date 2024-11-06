using Backbone.BuildingBlocks.API.Mvc;
using Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncementsInLanguage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.Modules.Announcements.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class AnnouncementsController : ApiControllerBase
{
    public AnnouncementsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAnnouncements([FromQuery] string language)
    {
        var announcements = await _mediator.Send(new GetAllAnnouncementsInLanguageQuery { Language = language });
        return Ok(announcements);
    }
}
