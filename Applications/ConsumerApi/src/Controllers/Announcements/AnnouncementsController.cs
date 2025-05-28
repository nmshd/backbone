using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Announcements.Application.Announcements.Queries.ListAllAnnouncementsInLanguage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.ConsumerApi.Controllers.Announcements;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class AnnouncementsController : ApiControllerBase
{
    public AnnouncementsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [ProducesResponseType<HttpResponseEnvelopeResult<ListAllAnnouncementsInLanguageResponse>>(StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListAllAnnouncements([FromQuery] string language)
    {
        var announcements = await _mediator.Send(new ListAllAnnouncementsForActiveIdentityInLanguageQuery { Language = language });
        return Ok(announcements);
    }
}
