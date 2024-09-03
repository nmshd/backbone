using Backbone.BuildingBlocks.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
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
