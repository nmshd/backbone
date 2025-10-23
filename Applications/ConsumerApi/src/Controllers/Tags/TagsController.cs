using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.ConsumerApi.Versions;
using Backbone.Modules.Tags.Application.Tags.Queries.ListTags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.ConsumerApi.Controllers.Tags;

[V2]
[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class TagsController : ApiControllerBase
{
    public TagsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ListTagsResponse>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    [HandleHttpCaching]
    public async Task<IActionResult> ListTags(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListTagsQuery(), cancellationToken);

        return Ok(response);
    }
}
