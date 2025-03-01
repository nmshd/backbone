using System.Security.Cryptography;
using System.Text.Json;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.Modules.Tags.Application.Tags.Queries.ListTags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeoSmart.Utils;

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
    public async Task<IActionResult> ListTags([FromHeader(Name = "If-None-Match")] string? ifNoneMatch, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListTagsQuery(), cancellationToken);
        var json = JsonSerializer.SerializeToUtf8Bytes(response);
        var responseHash = _hasher.ComputeHash(json);
        var hashString = UrlBase64.Encode(responseHash);
        var etag = $"\"{hashString}\"";

        Response.Headers.ETag = etag;

        if (ifNoneMatch != null && (ifNoneMatch.SequenceEqual(etag) || ifNoneMatch.SequenceEqual(hashString)))
            return StatusCode(StatusCodes.Status304NotModified);

        return Ok(response);
    }
}
