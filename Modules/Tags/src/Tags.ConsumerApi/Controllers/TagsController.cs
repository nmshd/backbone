using Backbone.BuildingBlocks.API.Mvc;
using Backbone.Modules.Tags.Application.Tags.Queries.ListTags;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.Modules.Tags.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
public class TagsController : ApiControllerBase
{
    public TagsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> ListTags(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListTagsQuery(), cancellationToken);
        return Ok(response);
    }
}
