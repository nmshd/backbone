using System.Security.Cryptography;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Tags.Application.Tags.Queries.ListTags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.Modules.Tags.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class TagsController : ApiControllerBase
{
    private readonly MD5 _hasher;

    public TagsController(IMediator mediator) : base(mediator)
    {
        _hasher = MD5.Create();
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
