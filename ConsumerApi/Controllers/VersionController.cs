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
    [ProducesResponseType(typeof(VersionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBackboneMajorVersion(CancellationToken cancellationToken)
    {
        var majorVersion = await _versionService.GetBackboneMajorVersion();
        return Ok(new VersionResponse { MajorVersion = majorVersion });
    }
}

public class VersionResponse
{
    public required string MajorVersion { get; set; }
}
