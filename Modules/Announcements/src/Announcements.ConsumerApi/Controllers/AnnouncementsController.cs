using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Announcements.Application.Announcements.Queries.GetAllAnnouncementsInLanguage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [ProducesResponseType<HttpResponseEnvelopeResult<GetAllAnnouncementsInLanguageResponse>>(StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAnnouncements([FromQuery] string language)
    {
        var announcements = await _mediator.Send(new GetAllAnnouncementsInLanguageQuery { Language = language });
        return Ok(announcements);
    }
}
