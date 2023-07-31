using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Tiers.Commands.CreateQuotaForIdentity;
using Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteQuotaForIdentity;
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
using GetIdentityQueryDevices = Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity.GetIdentityQuery;
using GetIdentityQueryQuotas = Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity.GetIdentityQuery;
using GetIdentityResponseDevices = Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity.GetIdentityResponse;
using GetIdentityResponseQuotas = Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity.GetIdentityResponse;

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
    public async Task<IActionResult> GetIdentities([FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
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

    [HttpDelete("{identityAddress}/Quotas/{individualQuotaId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteIndividualQuota([FromRoute] string identityAddress, [FromRoute] string individualQuotaId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteQuotaForIdentityCommand(identityAddress, individualQuotaId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{address}")]
    [ProducesResponseType(typeof(GetIdentityResponse), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIdentityByAddress([FromRoute] string address, CancellationToken cancellationToken)
    {
        var identity = await _mediator.Send<GetIdentityResponseDevices>(new GetIdentityQueryDevices(address), cancellationToken);
        var quotas = await _mediator.Send<GetIdentityResponseQuotas>(new GetIdentityQueryQuotas(address), cancellationToken);

        var response = new GetIdentityResponse
        {
            Address = identity.Address,
            ClientId = identity.ClientId,
            PublicKey = identity.PublicKey,
            TierId = identity.TierId,
            CreatedAt = identity.CreatedAt,
            IdentityVersion = identity.IdentityVersion,
            NumberOfDevices = identity.NumberOfDevices,
            Devices = identity.Devices,
            Quotas = quotas.Quotas
        };

        return Ok(response);
    }
}

public class CreateQuotaForIdentityRequest
{
    public string MetricKey { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}

public class GetIdentityResponse
{
    public string Address { get; set; }
    public string ClientId { get; set; }
    public byte[] PublicKey { get; set; }

    public string TierId { get; set; }

    public DateTime CreatedAt { get; set; }

    public byte IdentityVersion { get; set; }

    public int NumberOfDevices { get; set; }

    public IEnumerable<DeviceDTO> Devices { get; set; }

    public IEnumerable<QuotaDTO> Quotas { get; set; }
}
