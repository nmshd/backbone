using Backbone.BuildingBlocks.API.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.Modules.Announcements.ConsumerApi.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize("OpenIddict.Validation.AspNetCore")]
    public class AnnouncementsController : ApiControllerBase
    {
        public AnnouncementsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAnnouncement()
        {
            return Ok();
        }
    }
}
