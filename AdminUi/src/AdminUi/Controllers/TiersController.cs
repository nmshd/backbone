﻿using Backbone.AdminUi.Infrastructure.DTOs;
using Backbone.AdminUi.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;
using Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForTier;
using Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;
using Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backbone.AdminUi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class TiersController : ApiControllerBase
{
    private readonly AdminUiDbContext _adminUiDbContext;

    public TiersController(IMediator mediator, AdminUiDbContext adminUiDbContext) : base(mediator)
    {
        _adminUiDbContext = adminUiDbContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<List<TierOverview>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTiers(CancellationToken cancellationToken)
    {
        var tiers = await _adminUiDbContext.TierOverviews.ToListAsync(cancellationToken);
        return Ok(tiers);
    }

    [HttpGet("{tierId}")]
    [ProducesResponseType(typeof(TierDetailsDTO), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTierByIdAsync([FromRoute] string tierId, CancellationToken cancellationToken)
    {
        var tier = await _mediator.Send(new GetTierByIdQuery(tierId), cancellationToken);
        return Ok(tier);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateTierResponse), StatusCodes.Status201Created)]
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
        var command = new DeleteTierCommand(tierId);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("{tierId}/Quotas")]
    [ProducesResponseType(typeof(TierQuotaDefinitionDTO), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<CreatedResult> CreateTierQuota([FromRoute] string tierId, [FromBody] CreateQuotaForTierRequest request, CancellationToken cancellationToken)
    {
        var createdTierQuotaDefinition = await _mediator.Send(new CreateQuotaForTierCommand(tierId, request.MetricKey, request.Max, request.Period), cancellationToken);
        return Created(createdTierQuotaDefinition);
    }

    [HttpDelete("{tierId}/Quotas/{tierQuotaDefinitionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTierQuota([FromRoute] string tierId, [FromRoute] string tierQuotaDefinitionId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteTierQuotaDefinitionCommand(tierId, tierQuotaDefinitionId), cancellationToken);
        return NoContent();
    }
}

public class CreateQuotaForTierRequest
{
    public required string MetricKey { get; set; }
    public required int Max { get; set; }
    public required QuotaPeriod Period { get; set; }
}
