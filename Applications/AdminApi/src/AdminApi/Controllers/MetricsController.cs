using Backbone.AdminApi.Versions;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.Modules.Quotas.Application.Metrics.Queries.ListMetrics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ListMetricsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListMetrics(CancellationToken cancellationToken)
    {
        var metrics = await _mediator.Send(new ListMetricsQuery(), cancellationToken);
        return Ok(metrics);
    }
}
