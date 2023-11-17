using Backbone.BuildingBlocks.API.Mvc;
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

    [HttpGet]
    [ResponseCache(Duration = 1800, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "address" })]
    [ProducesResponseType(typeof(List<QuotaDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListIndividualQuotas(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListQuotasForIdentityQuery(), cancellationToken);

        return Ok(response);
    }
}
