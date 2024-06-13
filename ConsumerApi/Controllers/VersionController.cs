using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.ConsumerApi.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
public class VersionController : ApiControllerBase
{
    private readonly VersionService _versionService;

    public VersionController(VersionService versionService, IMediator mediator) : base(mediator)
    {
        _versionService = versionService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBackboneMajorVersion(CancellationToken cancellationToken)
    {
        var majorVersion = await _versionService.GetBackboneMajorVersion();
        if (majorVersion != null)
            return Ok(new { majorVersion });

        return NotFound(new { message = "Version not found" });
    }
}
