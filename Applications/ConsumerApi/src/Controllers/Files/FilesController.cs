using System.Net.Mime;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.ConsumerApi.Controllers.Files.DTOs;
using Backbone.ConsumerApi.Controllers.Files.DTOs.Validators;
using Backbone.Modules.Files.Application;
using Backbone.Modules.Files.Application.Files.Commands.ClaimFileOwnership;
using Backbone.Modules.Files.Application.Files.Commands.CreateFile;
using Backbone.Modules.Files.Application.Files.Commands.DeleteFile;
using Backbone.Modules.Files.Application.Files.Commands.RegenerateFileOwnershipToken;
using Backbone.Modules.Files.Application.Files.DTOs;
using Backbone.Modules.Files.Application.Files.Queries.GetFileContent;
using Backbone.Modules.Files.Application.Files.Queries.GetFileMetadata;
using Backbone.Modules.Files.Application.Files.Queries.ListFileMetadata;
using Backbone.Modules.Files.Application.Files.Queries.ValidateFileOwnershipToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NeoSmart.Utils;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.ConsumerApi.Controllers.Files;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class FilesController : ApiControllerBase
{
    private readonly ApplicationConfiguration _configuration;

    public FilesController(IMediator mediator, IOptions<ApplicationConfiguration> options) : base(mediator)
    {
        _configuration = options.Value;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateFileResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFile([FromForm] CreateFileDTO dto, CancellationToken cancellationToken)
    {
        var validationResult = await new CreateFileDTOValidator().ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(new ApplicationError(validationResult.Errors.First().ErrorCode, validationResult.Errors.First().ErrorMessage));

        var inputStream = new MemoryStream();

        await dto.Content.CopyToAsync(inputStream, cancellationToken);

        var command = new CreateFileCommand
        {
            FileContent = inputStream.ToArray(),
            ExpiresAt = dto.ExpiresAt,
            CipherHash = UrlBase64.Decode(dto.CipherHash),
            OwnerSignature = UrlBase64.Decode(dto.OwnerSignature),
            EncryptedProperties = UrlBase64.Decode(dto.EncryptedProperties)
        };

        var response = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(DownloadFile), new { fileId = response.Id }, response);
    }

    [HttpGet("{fileId}")]
    [ProducesResponseType<byte[]>(StatusCodes.Status200OK, MediaTypeNames.Application.Octet)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadFile(string fileId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetFileContentQuery { Id = fileId }, cancellationToken);
        return File(response.FileContent, MediaTypeNames.Application.Octet);
    }


    [HttpGet("{fileId}/Metadata")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FileMetadataDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFileMetadata(string fileId, CancellationToken cancellationToken)
    {
        var metadata = await _mediator.Send(new GetFileMetadataQuery { Id = fileId }, cancellationToken);
        return Ok(metadata);
    }

    [HttpPatch("{fileId}/RegenerateOwnershipToken")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RegenerateFileOwnershipTokenResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status403Forbidden)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegenerateFileOwnershipToken(string fileId, CancellationToken cancellationToken)
    {
        var regenerationResult = await _mediator.Send(new RegenerateFileOwnershipTokenCommand { FileId = fileId }, cancellationToken);
        return Ok(regenerationResult);
    }


    [HttpPatch("{fileId}/ClaimFileOwnership")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ClaimFileOwnershipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status403Forbidden)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClaimFileOwnership(string fileId, [FromBody] ClaimFileRequest claimRequest, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ClaimFileOwnershipCommand { FileId = fileId, OwnershipToken = claimRequest.FileOwnershipToken }, cancellationToken);
        return Ok(response);
    }

    [HttpPost("{fileId}/ValidateOwnershipToken")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ValidateFileOwnershipTokenResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status403Forbidden)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateOwnershipToken(string fileId, [FromBody] ValidateFileTokenRequest validateRequest, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ValidateFileOwnershipTokenQuery(validateRequest.FileOwnershipToken, fileId), cancellationToken);
        return Ok(response);
    }


    [HttpDelete("{fileId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFile(string fileId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteFileCommand { Id = fileId }, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListFileMetadataResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListFileMetadata([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<string> ids, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _configuration.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _configuration.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_configuration.Pagination.MaxPageSize));

        var fileMetadata = await _mediator.Send(new ListFileMetadataQuery(paginationFilter, ids), cancellationToken);
        return Paged(fileMetadata);
    }
}
