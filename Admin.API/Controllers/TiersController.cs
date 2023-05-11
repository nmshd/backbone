using Admin.API.Mvc;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;
using Backbone.Modules.Devices.Application.Tiers.Queries.ListTiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Admin.API.Controllers;

[Route("api/v1/[controller]")]
public class TiersController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public TiersController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator) {
        _options = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListTiersResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTiersAsync([FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;
        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var tiers = await _mediator.Send(new ListTiersQuery(paginationFilter), cancellationToken);
        return Paged(tiers);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateTierResponse), StatusCodes.Status201Created)]
    public async Task<CreatedResult> PostTiers([FromBody] CreateTierCommand command, CancellationToken cancellationToken)
    {
        var createdTier = await _mediator.Send(command, cancellationToken);
        return Created(createdTier);
    }
}
