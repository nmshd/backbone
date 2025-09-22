using Backbone.BuildingBlocks.API;
using Backbone.ConsumerApi.Versions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.ConsumerApi.Controllers;

[V1]
[V2]
[Route("api/v{v:apiVersion}/[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<VersionResult>), StatusCodes.Status200OK)]
    public IActionResult GetBackboneMajorVersion()
    {
        return Ok(new HttpResponseEnvelopeResult<VersionResult>(
            new VersionResult { MajorVersion = VersionService.GetBackboneMajorVersion() }));
    }
}

public class VersionResult
{
    public required string MajorVersion { get; set; }
}
