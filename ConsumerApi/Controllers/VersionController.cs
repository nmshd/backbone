using Backbone.ConsumerApi.Mvc;
using Backbone.Modules.Devices.Application.Clients.DTOs;
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
    public async Task<IActionResult> GetCurrentBackboneVersion(CancellationToken cancellationToken)
    {
        var majorVersion = await _versionService.GetCurrentBackboneVersion();
        if (majorVersion != null)
        {
            return Ok(new { majorVersion });
        }
        return NotFound(new { message = "Version not found" });
    }
}
