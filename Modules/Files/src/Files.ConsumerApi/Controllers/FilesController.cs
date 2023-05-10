using System.Net.Mime;
using Backbone.API;
using Backbone.Modules.Files.Application;
using Backbone.Modules.Files.Application.Files.Commands.CreateFile;
using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Application.Files.Queries.GetFileContent;
using Backbone.Modules.Files.Application.Files.Queries.GetFileMetadata;
using Backbone.Modules.Files.Application.Files.Queries.ListFileMetadata;
using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Files.ConsumerApi.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NeoSmart.Utils;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Files.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class FilesController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public FilesController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateFileResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile([FromForm] CreateFileDTO dto)
    {
        var inputStream = new MemoryStream();

        await dto.Content.CopyToAsync(inputStream);

        var command = new CreateFileCommand
        {
            FileContent = inputStream.ToArray(),
            ExpiresAt = dto.ExpiresAt,
            CipherHash = UrlBase64.Decode(dto.CipherHash),
            Owner = dto.Owner,
            OwnerSignature = dto.OwnerSignature == null ? null : UrlBase64.Decode(dto.OwnerSignature),
            EncryptedProperties = UrlBase64.Decode(dto.EncryptedProperties)
        };

        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(DownloadFile), new { fileId = response.Id }, response);
    }

    [HttpGet("{fileId}")]
    [Produces(MediaTypeNames.Application.Octet)]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FileContentResult>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadFile(FileId fileId)
    {
        var response = await _mediator.Send(new GetFileContentQuery { Id = fileId });
        return File(response.FileContent, MediaTypeNames.Application.Octet);
    }


    [HttpGet("{fileId}/metadata")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FileMetadataDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFileMetadata(FileId fileId)
    {
        var metadata = await _mediator.Send(new GetFileMetadataQuery { Id = fileId });
        return Ok(metadata);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListFileMetadataResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListFileMetadata([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<FileId> ids)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var fileMetadata = await _mediator.Send(new ListFileMetadataQuery(paginationFilter, ids));
        return Paged(fileMetadata);
    }
}