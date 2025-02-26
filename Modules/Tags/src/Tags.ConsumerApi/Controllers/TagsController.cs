using System.Security.Cryptography;
using System.Text.Json;
using Backbone.BuildingBlocks.API.Mvc;
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
    [AllowAnonymous]
    public async Task<IActionResult> ListTags([FromHeader(Name = "If-None-Match")] string? ifNoneMatch, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListTagsQuery(), cancellationToken);
        var json = JsonSerializer.SerializeToUtf8Bytes(response);
        var responseHash = _hasher.ComputeHash(json);
        var hashString = Convert.ToBase64String(responseHash);

        Response.Headers.ETag = hashString;

        if (ifNoneMatch != null && ifNoneMatch.SequenceEqual(hashString))
            return StatusCode(StatusCodes.Status304NotModified);

        return Ok(response);
    }
}
