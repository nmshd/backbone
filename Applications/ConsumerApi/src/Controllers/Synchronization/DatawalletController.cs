using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Synchronization.Application;
using Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetDatawallet;
using Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetModifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.ConsumerApi.Controllers.Synchronization;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class DatawalletController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public DatawalletController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<DatawalletDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetDatawalletQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("Modifications")]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<GetModificationsResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetModifications([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] int? localIndex,
        [FromHeader(Name = "X-Supported-Datawallet-Version")]
        ushort supportedDatawalletVersion,
        CancellationToken cancellationToken)
    {
        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        var request = new GetModificationsQuery(paginationFilter, localIndex, supportedDatawalletVersion);

        var response = await _mediator.Send(request, cancellationToken);
        return Paged(response);
    }

    [HttpPost("Modifications")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<PushDatawalletModificationsResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PushModifications(PushDatawalletModificationsRequestBody request,
        [FromHeader(Name = "X-Supported-Datawallet-Version")]
        ushort supportedDatawalletVersion, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new PushDatawalletModificationsCommand(request.Modifications,
            request.LocalIndex, supportedDatawalletVersion), cancellationToken);
        return Created(string.Empty, response);
    }
}

public class PushDatawalletModificationsRequestBody
{
    public required long LocalIndex { get; set; }
    public required PushDatawalletModificationItem[] Modifications { get; set; }
}
