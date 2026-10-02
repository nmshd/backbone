using FluentValidation;
using Backbone.Tooling.Extensions;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
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
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromForm] RequestFormParams dto, HttpContext context, IMediator mediator, CancellationToken cancellationToken)
    {
        ValidateRequiredFormFields(dto);
        var validationResult = await new RequestFormParamsValidator().ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            throw new Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ValidationException(new ApplicationError(validationResult.Errors.First().ErrorCode, validationResult.Errors.First().ErrorMessage));

        using var inputStream = new MemoryStream();
        await dto.Content.CopyToAsync(inputStream, cancellationToken);
        var response = await mediator.Send(new Command
        {
            FileContent = inputStream.ToArray(),
            ExpiresAt = dto.ExpiresAt,
            CipherHash = Base64Helper.Decode(dto.CipherHash),
            OwnerSignature = Base64Helper.Decode(dto.OwnerSignature),
            EncryptedProperties = Base64Helper.Decode(dto.EncryptedProperties)
        }, cancellationToken);
        return EnvelopeHttpResults.CreatedAtRoute(GetFileContent.Endpoint.ROUTE_NAME, new { v = context.Request.RouteValues["v"], fileId = response.Id }, response);
    }

    private static void ValidateRequiredFormFields(RequestFormParams formParams)
    {
        // Minimal API form mapping does not perform MVC's implicit required-field validation.
        if (formParams.Content == null)
            throw new BadHttpRequestException("'Content': The Content field is required.");
        foreach (var (name, value) in new[]
                 {
                     (nameof(formParams.OwnerSignature), formParams.OwnerSignature),
                     (nameof(formParams.CipherHash), formParams.CipherHash),
                     (nameof(formParams.EncryptedProperties), formParams.EncryptedProperties)
                 })
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BadHttpRequestException($"'{name}': The {name} field is required.");
        }
    }

    private class RequestFormParams
    {
        public required IFormFile Content { get; set; }

        public required string OwnerSignature { get; set; }

        public required string CipherHash { get; set; }

        public required DateTime ExpiresAt { get; set; }

        public required string EncryptedProperties { get; set; }
    }

    private class RequestFormParamsValidator : AbstractValidator<RequestFormParams>
    {
        private const string MIME_TYPE = "application/octet-stream";

        public RequestFormParamsValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(f => f.Content).NotNull();
            RuleFor(f => f.Content.Length).InclusiveBetween(1, 10.Mebibytes()).WithName("Content Length");
        }
    }
}
