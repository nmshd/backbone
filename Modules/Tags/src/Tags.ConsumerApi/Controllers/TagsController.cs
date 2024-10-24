using Backbone.BuildingBlocks.API.Mvc;
using Backbone.Modules.Tags.Application.Tags.Queries.ListTags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.Modules.Tags.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class TagsController : ApiControllerBase
{
    public TagsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ListTags(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListTagsQuery(), cancellationToken);
        return Ok(response);
    }
}
