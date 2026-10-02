using Backbone.AdminApi.Versions;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ListMetrics = Backbone.Modules.Quotas.Module.Features.Metrics.ListMetrics;

namespace Backbone.AdminApi.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("ApiKey")]
[V1]
public class MetricsController : ApiControllerBase
{
    public MetricsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ListMetrics.Response>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListMetrics(CancellationToken cancellationToken)
    {
        var metrics = await _mediator.Send(new ListMetrics.Query(), cancellationToken);
        return Ok(metrics);
    }
}
