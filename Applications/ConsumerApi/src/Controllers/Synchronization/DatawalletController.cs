using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.ConsumerApi.Versions;
using Backbone.Modules.Synchronization.Application;
using Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetDatawallet;
using Backbone.Modules.Synchronization.Application.Datawallets.Queries.ListModifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.ConsumerApi.Controllers.Synchronization;

[V2]
[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class DatawalletController : ApiControllerBase
{
    private readonly ApplicationConfiguration _configuration;

    public DatawalletController(IMediator mediator, IOptions<ApplicationConfiguration> options) : base(mediator)
    {
        _configuration = options.Value;
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
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListModificationsResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListModifications([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] int? localIndex,
        [FromHeader(Name = "X-Supported-Datawallet-Version")]
        ushort supportedDatawalletVersion,
        CancellationToken cancellationToken)
    {
        if (paginationFilter.PageSize > _configuration.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_configuration.Pagination.MaxPageSize));

        paginationFilter.PageSize ??= _configuration.Pagination.DefaultPageSize;

        var request = new ListModificationsQuery { PaginationFilter = paginationFilter, LocalIndex = localIndex, SupportedDatawalletVersion = supportedDatawalletVersion };

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
        var response = await _mediator.Send(
            new PushDatawalletModificationsCommand { Modifications = request.Modifications, LocalIndex = request.LocalIndex, SupportedDatawalletVersion = supportedDatawalletVersion },
            cancellationToken);
        return Created(string.Empty, response);
    }
}

public class PushDatawalletModificationsRequestBody
{
    public required long LocalIndex { get; set; }
    public required PushDatawalletModificationItem[] Modifications { get; set; }
}
