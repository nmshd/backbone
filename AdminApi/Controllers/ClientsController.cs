using AdminApi.Mvc;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Clients.Queries.ListClients;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace AdminApi.Controllers;

[Route("api/v1/[controller]")]

public class ClientsController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public ClientsController(
        IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    
    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListClientsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllClients([FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;
        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var identities = await _mediator.Send(new ListClientsQuery(paginationFilter), cancellationToken);
        return Paged(identities);
    }
}
