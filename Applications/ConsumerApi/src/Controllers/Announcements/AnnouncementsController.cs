using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.ConsumerApi.Versions;
using Backbone.Modules.Announcements.Application.Announcements.Queries.ListAnnouncementsInLanguage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.ConsumerApi.Controllers.Announcements;

[V2]
[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class AnnouncementsController : ApiControllerBase
{
    public AnnouncementsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [ProducesResponseType<HttpResponseEnvelopeResult<ListAnnouncementsInLanguageResponse>>(StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListAnnouncements([FromQuery] string language)
    {
        var announcements = await _mediator.Send(new ListAnnouncementsForActiveIdentityInLanguageQuery { Language = language });
        return Ok(announcements);
    }
}
