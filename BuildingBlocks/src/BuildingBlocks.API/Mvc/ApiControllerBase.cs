using System.Net.Mime;
using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.BuildingBlocks.API.Mvc;

[Produces(MediaTypeNames.Application.Json)]
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected readonly IMediator _mediator;

    protected ApiControllerBase(IMediator mediator)
    {
        _mediator = mediator;
    }

    [NonAction]
    public new IActionResult Ok()
    {
        return base.Ok(HttpResponseEnvelope.CreateSuccess());
    }

    [NonAction]
    public override OkObjectResult Ok(object? result)
    {
        return base.Ok(HttpResponseEnvelope.CreateSuccess(result));
    }

    [NonAction]
    public override CreatedResult Created(string? uri, object? result)
    {
        return base.Created(uri, HttpResponseEnvelope.CreateSuccess(result));
    }

    [NonAction]
    public CreatedResult Created(object result)
    {
        return base.Created("", HttpResponseEnvelope.CreateSuccess(result));
    }

    [NonAction]
    public override CreatedAtActionResult CreatedAtAction(string? actionName, object? result)
    {
        return base.CreatedAtAction(actionName, HttpResponseEnvelope.CreateSuccess(result));
    }

    [NonAction]
    public override CreatedAtActionResult CreatedAtAction(string? actionName, object? routeValues, object? result)
    {
        return base.CreatedAtAction(actionName, routeValues, HttpResponseEnvelope.CreateSuccess(result));
    }

    [NonAction]
    public ObjectResult PartialContent(object result)
    {
        return StatusCode(StatusCodes.Status206PartialContent, HttpResponseEnvelope.CreateSuccess(result));
    }

    public OkObjectResult Paged<T>(PagedResponse<T> response)
    {
        return Paged(response, response.Pagination);
    }

    public OkObjectResult Paged<T>(IEnumerable<T> pagedData, PaginationData paginationData)
    {
        if (paginationData.TotalPages <= 1) return Ok(pagedData);

        var response = new PagedHttpResponseEnvelope<T>(pagedData, new PagedHttpResponseEnvelopePaginationData
        {
            TotalRecords = paginationData.TotalRecords,
            PageNumber = paginationData.PageNumber,
            TotalPages = paginationData.TotalPages,
            PageSize = paginationData.PageSize
        });

        return new OkObjectResult(response);
    }
}
