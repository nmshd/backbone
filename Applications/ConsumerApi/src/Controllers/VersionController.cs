using Backbone.BuildingBlocks.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
public class VersionController : ControllerBase
{
    private readonly VersionService _versionService;

    public VersionController(VersionService versionService)
    {
        _versionService = versionService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<VersionResult>), StatusCodes.Status200OK)]
    public IActionResult GetBackboneMajorVersion(CancellationToken cancellationToken)
    {
        return Ok(new HttpResponseEnvelopeResult<VersionResult>(
            new VersionResult { MajorVersion = _versionService.GetBackboneMajorVersion() }));
    }
}

public class VersionResult
{
    public required string MajorVersion { get; set; }
}
