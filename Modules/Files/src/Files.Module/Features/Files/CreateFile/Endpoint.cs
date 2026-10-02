using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Files.Module.Features.Files.CreateFile.Http;
using Backbone.Tooling;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Files.Module.Features.Files.CreateFile;

internal static class Endpoint
{
    public static RouteGroupBuilder MapCreateFileEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .DisableAntiforgery()
            .Accepts<RequestFormParams>("multipart/form-data")
            .Produces<HttpResponseEnvelopeResult<CreateFileResponse>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromForm] RequestFormParams dto, HttpContext context, IMediator mediator, CancellationToken cancellationToken)
    {
        ValidateRequiredFormFields(dto);
        var validationResult = await new RequestFormParamsValidator().ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(new ApplicationError(validationResult.Errors.First().ErrorCode, validationResult.Errors.First().ErrorMessage));

        using var inputStream = new MemoryStream();
        await dto.Content.CopyToAsync(inputStream, cancellationToken);
        var response = await mediator.Send(new CreateFileCommand
        {
            FileContent = inputStream.ToArray(),
            ExpiresAt = dto.ExpiresAt,
            CipherHash = Base64Helper.Decode(dto.CipherHash),
            OwnerSignature = Base64Helper.Decode(dto.OwnerSignature),
            EncryptedProperties = Base64Helper.Decode(dto.EncryptedProperties)
        }, cancellationToken);
        return EnvelopeHttpResults.CreatedAtRoute(GetFileContent.Endpoint.ROUTE_NAME, new { v = context.Request.RouteValues["v"], fileId = response.Id }, response);
    }

    private static void ValidateRequiredFormFields(RequestFormParams dto)
    {
        // Minimal API form mapping does not perform MVC's implicit required-field validation.
        if (dto.Content == null)
            throw new BadHttpRequestException("'Content': The Content field is required.");
        foreach (var (name, value) in new[]
                 {
                     (nameof(dto.OwnerSignature), dto.OwnerSignature),
                     (nameof(dto.CipherHash), dto.CipherHash),
                     (nameof(dto.EncryptedProperties), dto.EncryptedProperties)
                 })
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BadHttpRequestException($"'{name}': The {name} field is required.");
        }
    }
}
