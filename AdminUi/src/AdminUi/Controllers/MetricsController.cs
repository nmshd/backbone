using Backbone.Modules.Quotas.Application.Metrics.Queries.ListMetrics;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminUi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class MetricsController : ApiControllerBase
{
    public MetricsController(IMediator mediator) : base(mediator) { }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ListMetricsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllMetrics(CancellationToken cancellationToken)
    {
        var metrics = await _mediator.Send(new ListMetricsQuery(), cancellationToken);
        return Ok(metrics);
    }
}
