using Enmeshed.BuildingBlocks.Application.FluentValidation;
using Enmeshed.Tooling.Extensions;
using FluentValidation;

namespace Backbone.API.DTOs.Validators;

public class CreateFileDTOValidator : AbstractValidator<CreateFileDTO>
{
    private const string MIME_TYPE = "application/octet-stream";

    public CreateFileDTOValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(f => f.Content).NotNull();
        RuleFor(f => f.Content.ContentType).In(MIME_TYPE).WithName("Content Type")
            .WithMessage($"The file must have the MIME type {MIME_TYPE}.");
        RuleFor(f => f.Content.Length).InclusiveBetween(1, 10.Mebibytes()).WithName("Content Length");
    }
}