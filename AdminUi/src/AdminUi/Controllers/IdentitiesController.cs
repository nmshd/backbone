using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Identities.Queries.GetIdentityByAddress;
using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentityQuotasByAddress;
using Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForIdentity;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;
using MetricDTO = Backbone.Modules.Devices.Application.DTOs.MetricDTO;
using QuotaDTO = Backbone.Modules.Devices.Application.DTOs.QuotaDTO;

namespace AdminUi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class IdentitiesController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public IdentitiesController(
        IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListIdentitiesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIdentitiesAsync([FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;
        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var identities = await _mediator.Send(new ListIdentitiesQuery(paginationFilter), cancellationToken);
        return Paged(identities);
    }

    [HttpPost("{identityAddress}/Quotas")]
    [ProducesResponseType(typeof(IndividualQuotaDTO), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<CreatedResult> CreateIndividualQuota([FromRoute] string identityAddress, [FromBody] CreateQuotaForIdentityRequest request, CancellationToken cancellationToken)
    {
        var createdIndividualQuotaDTO = await _mediator.Send(new CreateQuotaForIdentityCommand(identityAddress, request.MetricKey, request.Max, request.Period), cancellationToken);
        return Created(createdIndividualQuotaDTO);
    }

    [HttpGet("{address}")]
    [ProducesResponseType(typeof(GetIdentityByAddressResponse), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIdentityByAddressAsync([FromRoute] string address, CancellationToken cancellationToken)
    {
        var identity = await _mediator.Send(new GetIdentityByAddressQuery(address), cancellationToken);
        var identityWithQuotas = await _mediator.Send(new GetIdentityQuotasByAddressQuery(address), cancellationToken);

        var quotas = new List<QuotaDTO>();

        quotas.AddRange(identityWithQuotas.IndividualQuotas.Select(q =>
            new QuotaDTO(
                q.Id,
                "Individual",
                new MetricDTO(q.Metric.Key, q.Metric.DisplayName),
                q.Max,
                q.Period.ToString()
            )
        ));

        quotas.AddRange(identityWithQuotas.TierQuotas.Select(q =>
            new QuotaDTO(
                q.Id,
                "Tier",
                new MetricDTO(q.Metric.Key, q.Metric.DisplayName),
                q.Max,
                q.Period.ToString()
            )
        ));

        identity.Quotas = quotas;

        return Ok(identity);
    }
}

public class CreateQuotaForIdentityRequest
{
    public string MetricKey { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}
