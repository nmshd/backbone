using AdminUi.Infrastructure.DTOs;
using AdminUi.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;
using Backbone.Modules.Devices.Application.Tiers.Commands.DeleteTier;
using Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForTier;
using Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;
using Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace AdminUi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class TiersController : ApiControllerBase
{
    private readonly AdminUiDbContext _adminUiDbContext;
    private readonly ApplicationOptions _options;

    public TiersController(IMediator mediator, IOptions<ApplicationOptions> options, AdminUiDbContext adminUiDbContext) : base(mediator)
    {
        _adminUiDbContext = adminUiDbContext;
        _options = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<TierOverview>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTiersAsync([FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;
        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var tierOverviews = await _adminUiDbContext.TierOverviews.OrderAndPaginate(d => d.Name, paginationFilter, cancellationToken);
        return Paged(new PagedResponse<TierOverview>(tierOverviews.ItemsOnPage, paginationFilter, tierOverviews.TotalNumberOfItems));
    }

    [HttpGet("{tierId}")]
    [ProducesResponseType(typeof(GetTierByIdResponse), StatusCodes.Status200OK)]
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
    public string MetricKey { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}
