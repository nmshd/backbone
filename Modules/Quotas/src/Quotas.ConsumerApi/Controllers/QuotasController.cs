using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Quotas.Application.Identities.Queries.ListQuotasForIdentity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.Modules.Quotas.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class QuotasController : ApiControllerBase
{
    public QuotasController(IMediator mediator) : base(mediator) { }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ListQuotasForIdentityResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListIndividualQuotas(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListQuotasForIdentityQuery(), cancellationToken);

        return Ok(response);
    }
}
