using Enmeshed.BuildingBlocks.Application.Pagination;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Admin.API.Mvc;
using Microsoft.Extensions.Options;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Admin.API.Controllers;

[Route("api/v1/[controller]")]
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
}
