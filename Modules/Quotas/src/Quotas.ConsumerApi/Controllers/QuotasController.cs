using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Identities.Queries.GetQuotasForIdentity;
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

    [HttpGet("{address}")]
    [ResponseCache(Duration = 1800, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "address" })]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<List<QuotaDTO>>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListIndividualQuotas(IdentityAddress address, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListQuotasForIdentityQuery(address), cancellationToken);

        return Ok(response);
    }
}
