using Backbone.AdminApi.DTOs;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.AdminApi.Versions;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;
using Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForTier;
using Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;
using Backbone.Modules.Quotas.Application.Tiers.Queries.GetTier;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backbone.AdminApi.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("ApiKey")]
[V1]
public class TiersController : ApiControllerBase
{
    private readonly AdminApiDbContext _adminApiDbContext;

    public TiersController(IMediator mediator, AdminApiDbContext adminApiDbContext) : base(mediator)
    {
        _adminApiDbContext = adminApiDbContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<List<TierOverviewDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListTiers(CancellationToken cancellationToken)
    {
        var tiers = await _adminApiDbContext.Tiers.Select(t => new TierOverviewDTO
        {
            Id = t.Id,
            Name = t.Name,
            NumberOfIdentities = _adminApiDbContext.Identities.Count(i => i.TierId == t.Id),
            CanBeManuallyAssigned = t.CanBeManuallyAssigned,
            CanBeUsedAsDefaultForClient = t.CanBeUsedAsDefaultForClient
        }).ToListAsync(cancellationToken);

        return Ok(tiers);
    }

    [HttpGet("{tierId}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<TierDetailsDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTier([FromRoute] string tierId, CancellationToken cancellationToken)
    {
        var tier = await _mediator.Send(new GetTierQuery { Id = tierId }, cancellationToken);
        return Ok(tier);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateTierResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<CreatedResult> PostTiers([FromBody] CreateTierCommand command, CancellationToken cancellationToken)
    {
        var createdTier = await _mediator.Send(command, cancellationToken);
        return Created(createdTier);
    }

    [HttpDelete("{tierId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteTier([FromRoute] string tierId, CancellationToken cancellationToken)
    {
        var command = new DeleteTierCommand { TierId = tierId };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("{tierId}/Quotas")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<TierQuotaDefinitionDTO>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<CreatedResult> CreateTierQuota([FromRoute] string tierId, [FromBody] CreateQuotaForTierRequest request, CancellationToken cancellationToken)
    {
        var createdTierQuotaDefinition = await _mediator.Send(new CreateQuotaForTierCommand { TierId = tierId, MetricKey = request.MetricKey, Max = request.Max, Period = request.Period },
            cancellationToken);
        return Created(createdTierQuotaDefinition);
    }

    [HttpDelete("{tierId}/Quotas/{tierQuotaDefinitionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTierQuota([FromRoute] string tierId, [FromRoute] string tierQuotaDefinitionId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteTierQuotaDefinitionCommand { TierId = tierId, TierQuotaDefinitionId = tierQuotaDefinitionId }, cancellationToken);
        return NoContent();
    }
}

public class CreateQuotaForTierRequest
{
    public required string MetricKey { get; set; }
    public required int Max { get; set; }
    public required QuotaPeriod Period { get; set; }
}
